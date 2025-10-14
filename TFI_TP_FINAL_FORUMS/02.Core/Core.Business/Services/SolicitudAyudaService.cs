using Core.Contracts.Repositories;
using Core.Contracts.Services;
using Core.Contracts.UoW;
using Core.Domain.Enum;
using Core.Domain.Exceptions.BaseException;
using Core.Domain.GenericEntityClass;
using Core.Domain.IdentityModels;
using Core.Domain.Models;
using Core.Domain.Request;
using Core.Domain.Specification;
using Core.Domain.Specification.Business;
using CrossCutting.Extensions.Linq;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Core.Business.Services
{
    public class SolicitudAyudaService : GenericService<SolicitudAyudaModel>, ISolicitudAyudaService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IUnitOfWorkForum _unitOfWorkForum;
        private readonly IUnitOfWorkGateway _unitOfWorkGateway;
        public SolicitudAyudaService(
            IUnitOfWorkForum unitOfWork,
            IUnitOfWorkGateway unitOfWorkGateway
        )
        : base(unitOfWork, unitOfWork.GetRepository<ISolicitudAyudaRepository>())
        {
            _usersRepository = unitOfWorkGateway.GetRepository<IUsersRepository>();
            _unitOfWorkForum = unitOfWork;
            _unitOfWorkGateway = unitOfWorkGateway;
        }

        public async Task<CursorPage<SolicitudAyudaModel>> GetRequestsHelp(
            int limit,
            DateTime anchorUtc,
            (DateTime createdAt, int id)? after,
            string? userId,
            string? search
        )
        {
            const string Collation = "Latin1_General_100_CI_AI";

            var userFiltersRepository = _unitOfWorkGateway.GetRepository<IUserFiltersRepository>();
            var groupRepository = _unitOfWorkGateway.GetRepository<IGroupRepository>();

            var allUserFilters = (await userFiltersRepository
                .Get(x => x.UserId == userId, includeProperties: "Filter,Filter.FilterType"))
                .ToList();

            var groupFilterIds = (await groupRepository
                .Get(x => x.Name == GroupEnum.ForumRequestHelp.ToString(), includeProperties: "GroupFilters"))
                .First()
                .GroupFilters
                .Select(y => y.IDFilter)
                .ToHashSet();

            var userFiltersRequest = allUserFilters.Where(x => groupFilterIds.Contains(x.IDFilter));

            // Mapa nombre → valor
            var map = ToFilterMap(userFiltersRequest);

            // Base: vigentes a la fecha (tu spec)
            var validateAnchor = new RequestHelpValidAtSpec(anchorUtc);

            var q = _repository.Query(
                validateAnchor, // condición base (CreateDate<=anchor && Vence>=anchor)
                includeProperties: "SolicitudAyudaEstado,SolicitudAyudaEtiquetas.Etiqueta,Disponibilidades",
                tracking: false
            );

            // Filtro por usuario (excluir autor)
            if (!string.IsNullOrWhiteSpace(userId))
                q = q.Where(s => s.IDUsuarioSolicitante != userId);

            // Cursor (keyset)
            if (after.HasValue)
            {
                var a = after.Value;
                q = q.Where(s => s.CreateDate < a.createdAt
                              || (s.CreateDate == a.createdAt && s.IDSolicitudAyuda < a.id));
            }

            // Search textual
            if (!string.IsNullOrWhiteSpace(search))
            {
                var like = $"%{EscapeLike(search)}%";
                q = q.Where(s =>
                    EF.Functions.Like(EF.Functions.Collate(s.Titulo, Collation), like) ||
                    EF.Functions.Like(EF.Functions.Collate(s.Descripcion, Collation), like) ||
                    s.SolicitudAyudaEtiquetas.Any(e =>
                        EF.Functions.Like(EF.Functions.Collate(e.Etiqueta.NombreEtiqueta, Collation), like))
                );
            }

            // ⬇️ Aplicamos los filtros del usuario (puede incluir ORDER BY)
            q = ApplyUserFilters(q, map, Collation);

            // Si el usuario no tenía OrderBy guardado, garantizamos un orden consistente para el cursor
            // (solo si la estrategia anterior no aplicó ningún OrderBy)
            if (!(q.Expression.ToString().Contains("OrderBy") || q.Expression.ToString().Contains("OrderByDescending")))
            {
                q = q.OrderByDescending(s => s.CreateDate)
                     .ThenByDescending(s => s.IDSolicitudAyuda);
            }

            // Page fetch (+1)
            var items = q.Take(limit + 1).ToList();

            var hasNext = items.Count > limit;
            if (hasNext) items.RemoveAt(items.Count - 1);

            string? nextCursor = null;
            if (hasNext && items.Count > 0)
            {
                var last = items[^1];
                var payload = $"{last.CreateDate.Ticks}:{last.IDSolicitudAyuda}";
                nextCursor = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(payload));
            }

            // Lookup de usuarios
            var ids = items.Select(p => p.IDUsuarioSolicitante).Distinct().ToList();
            var usuarios = (await _usersRepository
                .Get(x => ids.Contains(x.Id), includeProperties: "UsersForum", tracking: false))
                .ToDictionary(u => u.Id);

            foreach (var s in items)
                if (usuarios.TryGetValue(s.IDUsuarioSolicitante, out var user))
                    s.UsuarioSolicitante = user;

            return new CursorPage<SolicitudAyudaModel>
            {
                Items = items,
                NextCursor = nextCursor,
                HasNext = hasNext
            };
        }

        public async Task<bool> CreateHelpRequest(CreateHelpRequest request)
        {
            if (request == null)
                throw new ApiForumException("Solicitud inválida.");

            if (string.IsNullOrWhiteSpace(request.TitleHelp) || string.IsNullOrWhiteSpace(request.UserId))
                throw new ApiForumException("Datos insuficientes para crear la solicitud de ayuda.");

            var user = (await _usersRepository.Get(x => x.Id == request.UserId, tracking: false)).FirstOrDefault();
            if (user == null)
                throw new ApiForumException("No existe el usuario.");

            var nowUtc = DateTime.UtcNow;

            // Estado inicial: "Activa"; si no existe, lo creamos
            var estadoRepo = _unitOfWorkForum.GetRepository<ISolicitudAyudaEstadoRepository>();
            var estadoActiva = (await estadoRepo.Get(x => x.Estado == "Activa", tracking: true)).First();

            var solicitud = new SolicitudAyudaModel
            {
                Titulo = request.TitleHelp.Trim(),
                Descripcion = request.Message?.Trim(),
                IDUsuarioSolicitante = user.Id,
                CreateDate = nowUtc,
                IDEstado = estadoActiva.IDEstado,
                SolicitudAyudaEstado = estadoActiva,
                Lenguaje = request.Languages != null && request.Languages.Count > 0
                    ? string.Join(",", request.Languages.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()))
                    : null,
                RecompensaBase = 1m,
                IncrementoPorHora = 0.5m,
                FechaVencimiento = nowUtc.AddHours(48)
            };

            // Etiquetas
            if (request.Labels != null && request.Labels.Count > 0)
            {
                var labelNames = request.Labels
                    .Where(n => !string.IsNullOrWhiteSpace(n))
                    .Select(n => n.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (labelNames.Count > 0)
                {
                    var etiquetaRepo = _unitOfWorkForum.GetRepository<IEtiquetaRepository>();
                    var etiquetas = (await etiquetaRepo.Get(e => labelNames.Contains(e.NombreEtiqueta), tracking: true)).ToList();
                    foreach (var et in etiquetas)
                    {
                        solicitud.SolicitudAyudaEtiquetas.Add(new SolicitudAyudaEtiquetasModel
                        {
                            IDEtiqueta = et.IDEtiqueta
                        });
                    }
                }
            }

            // Disponibilidades
            if (request.TimeSlot?.Slots != null && request.TimeSlot.Slots.Count > 0)
            {
                foreach (var slot in request.TimeSlot.Slots)
                {
                    if (DateTime.TryParse(slot.Start, null, DateTimeStyles.RoundtripKind, out var inicio) &&
                        DateTime.TryParse(slot.End, null, DateTimeStyles.RoundtripKind, out var fin) &&
                        fin > inicio)
                    {
                        solicitud.Disponibilidades.Add(new SolicitudAyudaDisponibilidadModel
                        {
                            Inicio = inicio, //Se queda en UTC
                            Fin = fin, //Se queda en UTC
                            Estado = 1
                        });
                    }
                }
            }

            await _repository.Insert(solicitud);
            return await _unitOfWorkForum.Complete();
        }

        public async Task<(SolicitudAyudaModel, int?)> GetDetailRequestsHelp(int codeRequest, string codeUser)
        {
            var requestHelp = (await _repository.Get(x=> x.IDSolicitudAyuda == codeRequest, includeProperties: "SolicitudAyudaEstado,SolicitudAyudaEtiquetas.Etiqueta,Disponibilidades,Chats")).FirstOrDefault();
            if (requestHelp == null)
                throw new ApiForumException("No se encontró la solicitud de ayuda indicada.");

            requestHelp.UsuarioSolicitante = (await _usersRepository.Get(x => x.Id == requestHelp.IDUsuarioSolicitante, tracking: false, includeProperties: "UsersForum")).FirstOrDefault();

            var chat = requestHelp.Chats.FirstOrDefault(c => c.IDUsuarioAyudante == codeUser && c.Active);
            return (requestHelp, chat?.IDChat);
        }

        public async Task<List<SolicitudAyudaModel>> GetMyRequestsHelp(string? userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return [];

            var q = _repository.Query(
                s => s.IDUsuarioSolicitante == userId && s.FechaVencimiento >= DateTime.UtcNow,
                includeProperties: "SolicitudAyudaEstado,SolicitudAyudaEtiquetas.Etiqueta,Disponibilidades",
                tracking: false
            ).OrderByDescending(s => s.CreateDate).ThenByDescending(s => s.IDSolicitudAyuda);

            return await q.ToListAsync();
        }

        public async Task<bool> UpdateDisponibility(int id, UpdateDisponibilityRequest request)
        {
            var requestHelp = (await _repository.Get(x => x.IDSolicitudAyuda == id && x.IDUsuarioSolicitante == request.UserId, 
                includeProperties: "Disponibilidades", tracking: true)).FirstOrDefault();
            if (requestHelp == null)
                throw new ApiForumException("No se encontró la solicitud de ayuda indicada o no tenés permisos para modificarla.");

            // Reemplazamos las disponibilidades actuales por las nuevas
            requestHelp.Disponibilidades.Clear();
            if (request.TimeSlot?.Slots != null && request.TimeSlot.Slots.Count > 0)
            {
                foreach (var slot in request.TimeSlot.Slots)
                {
                    if (DateTime.TryParse(slot.Start, null, DateTimeStyles.RoundtripKind, out var inicio) &&
                        DateTime.TryParse(slot.End, null, DateTimeStyles.RoundtripKind, out var fin) &&
                        fin > inicio)
                    {
                        requestHelp.Disponibilidades.Add(new SolicitudAyudaDisponibilidadModel
                        {
                            Inicio = inicio, //Se queda en UTC
                            Fin = fin, //Se queda en UTC
                            Estado = 1
                        });
                    }
                }
            }
            return await _unitOfWorkForum.Complete();
        }

        #region Helpers for GetRequestsHelp
        private static string EscapeLike(string input)
        {
            // Escapa comodines especiales de LIKE para evitar falsos positivos
            return input
                .Replace("[", "[[]")
                .Replace("%", "[%]")
                .Replace("_", "[_]")
                .Replace("]", "[]]");
        }

        private static Dictionary<string, string> ToFilterMap(IEnumerable<UserFiltersModel> userFilters)
        {
            // Normalizamos el nombre y tomamos último valor guardado si hubiera repetidos
            return userFilters
                .GroupBy(f => f.Filter.Name)
                .ToDictionary(g => g.Key, g => g.Last().Value ?? string.Empty, StringComparer.OrdinalIgnoreCase);
        }

        private static IQueryable<SolicitudAyudaModel> ApplyUserFilters(
            IQueryable<SolicitudAyudaModel> q,
            Dictionary<string, string> map,
            string collation // "Latin1_General_100_CI_AI"
        )
        {
            // ---- ForumRequestHelpOrderBy (SELECT: "Mas reciente,Mas antiguo") ----
            if (map.TryGetValue("ForumRequestHelpOrderBy", out var orderByRaw))
            {
                var order = orderByRaw?.Trim().ToLowerInvariant();
                // quitamos un ORDER BY default aquí; lo aplicaremos afuera con esta preferencia
                q = order switch
                {
                    "mas antiguo" => q.OrderBy(s => s.CreateDate).ThenBy(s => s.IDSolicitudAyuda),
                    _ => q.OrderByDescending(s => s.CreateDate).ThenByDescending(s => s.IDSolicitudAyuda) // default "Más reciente"
                };
            }

            // ---- ForumRequestHelpDateExpires (DATE) ----
            // Convención: el Value viene como "yyyy-MM-dd" o ISO; si viene algo relativo, lo podés adaptar.
            if (map.TryGetValue("ForumRequestHelpDateExpires", out var dateRaw)
                && DateTime.TryParse(dateRaw, out var dateLimit))
            {
                // Ej.: mostrar solicitudes cuyo vencimiento sea >= dateLimit
                q = q.Where(s => s.FechaVencimiento >= dateLimit);
            }

            // ---- ForumRequestHelpLabels (STRING, CSV o una sola) ----
            // Convención: guardás el Value como "csharp,react,api" o un único valor.
            if (map.TryGetValue("ForumRequestHelpLabels", out var labelsRaw) && !string.IsNullOrWhiteSpace(labelsRaw))
            {
                var labels = labelsRaw
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (labels.Count > 0)
                {
                    q = q.Where(s =>
                        s.SolicitudAyudaEtiquetas.Any(e =>
                            labels.Contains(
                                EF.Functions.Collate(e.Etiqueta.NombreEtiqueta, collation)
                            )));
                }
            }

            // ---- ForumRequestHelpLanguages (SELECT, CSV: "es-AR,en-US") ----
            // Si "Lenguajes" significa etiquetas de stack (JS, C#, etc.), filtrá igual que labels.
            // Si es cultura/idioma del pedido, ajustá la propiedad real (ej.: s.Idioma o s.Cultura).
            if (map.TryGetValue("ForumRequestHelpLanguages", out var langsRaw) && !string.IsNullOrWhiteSpace(langsRaw))
            {
                var langs = langsRaw
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (langs.Count > 0)
                {
                    // Opción B (comenta A y descomenta B) si tenés un campo idioma/cultura:
                    q = q.Where(s => s.Lenguaje != null && langs.Any(x => EF.Functions.Like("," + s.Lenguaje + ",", "%," + x + ",%")));
                }
            }

            return q;
        }
        #endregion
    }
}
