using Core.Contracts.Repositories;
using Core.Contracts.Services;
using Core.Domain.IdentityModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;
using Core.Contracts.UoW;
using Core.Domain.Specification;
using CrossCutting.Extensions.Linq;
using Core.Domain.Models;
using Core.Domain.GenericEntityClass;
using Core.Domain.Exceptions.BaseException;
using Core.Domain.Request;
using Core.Domain.Response;

namespace Core.Business.Services
{
    public class UsersService : GenericService<Users>, IUsersService
    {
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWorkForum _unitOfWorkForum;
        private readonly IUnitOfWorkGateway _unitOfWorkGateway;

        public UsersService(
            IUnitOfWorkGateway unitOfWorkGateway,
            IUnitOfWorkForum unitOfWorkForum,
            IMapper mapper,
            IEmailService emailService,
            IHttpContextAccessor httpContextAccessor)
            : base(unitOfWorkGateway, unitOfWorkGateway.GetRepository<IUsersRepository>())
        {
            _emailService = emailService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWorkForum = unitOfWorkForum;
            _unitOfWorkGateway = unitOfWorkGateway;
        }      
        
        public async Task<List<Users>> GetUsersAsync()
        {
            return (await _repository.Get(tracking: false)).ToList();            
        }       
            
        public async Task<bool> UpdateUserAsync(Users user)
        {
            try
            {
                //get user
                var userDb = (await _repository.Get(x => x.Id == user.Id)).FirstOrDefault();
                userDb.Active = true;
                userDb.Email = user.Email;
                userDb.FirstName = user.FirstName;
                userDb.LastName = user.LastName;
                userDb.PhoneNumber = user.PhoneNumber;
                userDb.UserName = user.UserName;
                await _repository.Update(userDb);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }     
        }

        public async Task<Users> GetUserByNameAsync(string userName)
        {
            return (await _repository.Get(x => x.UserName == userName, tracking: false)).FirstOrDefault();
        }
        
        public async Task<Dictionary<Users,int>> GetTopLastWeekAsync()
        {
            var DictTopUsersLastWeek = new Dictionary<Users, int>();
            try
            {
                var totalUsers = await _repository.Get();
                //Obtenemos el repositorio de recompensas de usuarios
                var usuariosRecompensasRepo = _unitOfWorkForum.GetRepository<IRecompensaUsuarioRepository>();

                var topUsers = await usuariosRecompensasRepo.GetTopThreeUsersLastWeek();

                foreach (var a in topUsers)
                {
                    var user = (await _repository.Get(x => x.Id == a.IDUsuario, includeProperties: "UsersForum", tracking: false)).FirstOrDefault();
                    if (user is not null)
                        DictTopUsersLastWeek.Add(user, a.TotalRecompensa);
                }

                return DictTopUsersLastWeek;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<(PaginatedList<Users>, Dictionary<Users, int>)> GetUsersForumAsync(int pageIndex, int pageCount, string userId)
        {
            try
            {
                var userFiltersRepository = _unitOfWorkGateway.GetRepository<IUserFiltersRepository>();
                var userFilters = (await userFiltersRepository.Get(x => x.UserId == userId, includeProperties: "Filter")).ToList();

                Specification<Users> combinedSpecification = new AdHocSpecification<Users>(user => true);

                foreach (var userFilter in userFilters)
                {
                    var newSpec = new AdHocSpecification<Users>(ExpressionExtensions.CreateContainsExpression<Users>(userFilter.Filter.Description, userFilter.Value));
                    combinedSpecification &= newSpec;
                }

                var paged = await _repository.GetPagedElements(
                    pageIndex,
                    pageCount,
                    orderByExpression: p => p.FirstName, 
                    ascending: false,
                    combinedSpecification,
                    includeProperties: "UsersForum",
                    tracking: false
                );

                Dictionary<Users, int> dicUsers = new Dictionary<Users, int>();

                var users = paged.List.Distinct().ToList();
                users.ForEach(async x =>
                {
                    Specification<RecompensaUsuarioModel> regardUser = new AdHocSpecification<RecompensaUsuarioModel>(reg => reg.IDUsuario == x.Id);
                    var recompensaUsuario = (await _unitOfWorkForum.GetRepository<IRecompensaUsuarioRepository>().Get(regardUser,includeProperties: "Recompensa")).Sum(x => x.Recompensa.Valor);
                    dicUsers.Add(x, recompensaUsuario);
                });

                return (paged,dicUsers);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Users> GetDetailUserAsync(string userEmail)
        {
            try
            {
                var userForum = (await _repository.Get(x=> x.Email == userEmail, includeProperties: "UsersForum")).FirstOrDefault();

                if(userForum is not null)
                {
                    userForum.RecompensasUsuarios = (await _unitOfWorkForum.GetRepository<IRecompensaUsuarioRepository>().Get(x => x.IDUsuario == userForum.Id)).ToList();
                    userForum.UsuarioMedallas = (await _unitOfWorkForum.GetRepository<IUsuarioMedallaRepository>().Get(x => x.IDUsuario == userForum.Id, includeProperties: "Medalla")).ToList();
                    userForum.Respuestas = (await _unitOfWorkForum.GetRepository<IRespuestaRepository>().Get(x => x.IDUsuario == userForum.Id)).ToList();
                    userForum.Publicaciones = (await _unitOfWorkForum.GetRepository<IPublicacionRepository>().Get(x => x.IDUsuario == userForum.Id)).ToList();
                }

                return userForum; ////////////////////////////////
        }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<List<NotificacionesModel>> GetNotificationsAsync(string userId)
        {
            try
            {
                var user = (await _repository.Get(x => x.Id == userId, includeProperties: "UsersForum")).FirstOrDefault();

                if (user is null)
                    throw new ApiForumException("El usuario no existe.");
                else
                {
                    return (await _unitOfWorkForum.GetRepository<INotificacionRepository>().Get(x => x.IDUsuario == userId, orderBy: y=> y.OrderByDescending(u=> u.FechaNotificacion)))?.ToList() ?? new List<NotificacionesModel>();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        // Campos estáticos para evitar reallocs por invocación
        private static readonly IReadOnlyDictionary<string, int> SeverityRank = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            ["critical"] = 0,
            ["high"] = 1,
            ["warning"] = 2,
            ["info"] = 3
        };

        private static readonly IReadOnlyDictionary<string, int> TypeRank = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            ["meeting_live"] = 0,
            ["meeting_soon"] = 1,
            ["request_expiring"] = 2,
            ["no_slots"] = 3,
            ["generic"] = 4
        };

        public async Task<IReadOnlyList<AlertResponse>> GetAlertsAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ApiForumException("Debe indicar el usuario.");

            var normalizedUser = userId.Trim();
            var nowUtc = DateTime.UtcNow;

            static int GetSeverityRank(string severity) =>
                SeverityRank.TryGetValue(severity, out var rank) ? rank : SeverityRank["info"];

            static int GetTypeRank(string type) =>
                TypeRank.TryGetValue(type, out var rank) ? rank : TypeRank["generic"];

            static DateTime EnsureUtc(DateTime value) =>
                value.Kind switch
                {
                    DateTimeKind.Unspecified => DateTime.SpecifyKind(value, DateTimeKind.Utc),
                    DateTimeKind.Local => value.ToUniversalTime(),
                    _ => value
                };

            static string FormatFutureDuration(TimeSpan span)
            {
                if (span < TimeSpan.Zero) span = TimeSpan.Zero;
                // Opción A: mantener días/horas/minutos pero truncando (coincide con el front en la idea de truncar)
                if (span.TotalDays >= 1) return $"{Math.Max(1, (int)Math.Floor(span.TotalDays))}d";
                if (span.TotalHours >= 1) return $"{Math.Max(1, (int)Math.Floor(span.TotalHours))}h";
                return $"{Math.Max(1, (int)Math.Floor(span.TotalMinutes))}m";
            }


            static string FormatPastDuration(TimeSpan span)
            {
                if (span < TimeSpan.Zero) span = TimeSpan.Zero;
                if (span.TotalDays >= 1) return $"{Math.Max(1, (int)Math.Floor(span.TotalDays))}d";
                if (span.TotalHours >= 1) return $"{Math.Max(1, (int)Math.Floor(span.TotalHours))}h";
                return $"{Math.Max(1, (int)Math.Floor(span.TotalMinutes))}m";
            }

            static bool IsRequestActiveState(string? state)
            {
                if (string.IsNullOrWhiteSpace(state)) return true;
                return !state.Equals("Cerrada", StringComparison.OrdinalIgnoreCase)
                    && !state.Equals("Cancelada", StringComparison.OrdinalIgnoreCase)
                    && !state.Equals("Expirada", StringComparison.OrdinalIgnoreCase);
            }

            var userExists = (await _repository.Get(x => x.Id == normalizedUser, tracking: false))?.Any() ?? false;
            if (!userExists)
                throw new ApiForumException("El usuario no existe.");

            var solicitudRepo = _unitOfWorkForum.GetRepository<ISolicitudAyudaRepository>();
            var reservaRepo = _unitOfWorkForum.GetRepository<ISolicitudAyudaReservaRepository>();

            var myRequests = (await solicitudRepo.Get(
                s => s.IDUsuarioSolicitante == normalizedUser && s.Active,
                includeProperties: "SolicitudAyudaEstado,Disponibilidades,Disponibilidades.Reservas,Disponibilidades.Reservas.Sesion,Chats",
                tracking: false)).ToList();

            var helperReservations = (await reservaRepo.Get(
                r => r.IDUsuarioAyudante == normalizedUser && r.Active,
                includeProperties: "Sesion,Disponibilidad,Disponibilidad.Solicitud,Disponibilidad.Solicitud.SolicitudAyudaEstado,Disponibilidad.Solicitud.Chats",
                tracking: false)).ToList();

            // Índices de chat (requestId + helperId) -> chatId
            var chatIndex = new Dictionary<(int RequestId, string HelperId), int>();
            void IndexChats(IEnumerable<SolicitudAyudaChatModel>? chats)
            {
                if (chats is null) return;
                foreach (var chat in chats)
                {
                    if (chat is null || !chat.Active) continue;
                    chatIndex[(chat.IDSolicitudAyuda, chat.IDUsuarioAyudante)] = chat.IDChat;
                }
            }

            foreach (var request in myRequests)
                IndexChats(request?.Chats);
            foreach (var reservation in helperReservations)
                IndexChats(reservation?.Disponibilidad?.Solicitud?.Chats);

            var alerts = new List<(AlertResponse Alert, int Severity, DateTime SortCreatedAt, DateTime? MeetingStart, int Type)>(capacity: 16);
            var seenMeetings = new HashSet<Guid>();

            void ProcessReservation(SolicitudAyudaModel? request, SolicitudAyudaReservaModel reservation)
            {
                if (request is null || reservation is null || !reservation.Active) return;

                var session = reservation.Sesion;
                if (session is null || !session.Active) return;

                if (!seenMeetings.Add(session.IDSesion)) return;

                var startUtc = EnsureUtc(session.Reserva.Disponibilidad.Inicio);
                var endUtc = EnsureUtc(session.Reserva.Disponibilidad.Fin);

                //var isLive = reservation.Estado == 2 && startUtc <= nowUtc && nowUtc <= expiresAtUtc;
                var isLive = startUtc <= nowUtc && nowUtc <= endUtc;
                var isSoon = !isLive
                          && startUtc >= nowUtc
                          && startUtc <= nowUtc.AddMinutes(10)
                          && reservation.Estado is not (3 or 4 or 5);

                var chatId = chatIndex.TryGetValue((request.IDSolicitudAyuda, reservation.IDUsuarioAyudante), out var chatCode)
                    ? chatCode
                    : (int?)null;

                var ctaHref = $"/forum/liveHelp/meeting/{session.IDSesion}";

                if (!isLive && !isSoon)
                {
                    // Missed (dentro de 1 hora de terminada)
                    if (endUtc <= nowUtc && nowUtc - endUtc <= TimeSpan.FromHours(1))
                    {
                        var missedAlert = new AlertResponse
                        {
                            Id = $"meeting:{session.IDSesion}:missed",
                            DedupeKey = $"{session.IDSesion}:missed",
                            Type = "generic",
                            Severity = "info",
                            Title = $"Te perdiste la reunión '{request.Titulo}'",
                            Message = $"Terminó hace {FormatPastDuration(nowUtc - endUtc)}. Revisá la solicitud para ponerte al día.",
                            CreatedAt = nowUtc,
                            ExpiresAt = endUtc.AddHours(12),
                            Sticky = false,
                            ChannelSuggested = "toast",
                            Cta = new AlertCtaResponse
                            {
                                Label = "Ver solicitud",
                                Href = $"/forum/liveHelp/detail/{request.IDSolicitudAyuda}"
                            },
                            Data = new
                            {
                                requestId = request.IDSolicitudAyuda,
                                meetingId = session.IDSesion,
                                chatId,
                                startTime = startUtc,
                                endTime = endUtc,
                                helperId = reservation.IDUsuarioAyudante,
                                requesterId = request.IDUsuarioSolicitante,
                                kind = "meeting_missed"
                            }
                        };

                        alerts.Add((missedAlert, GetSeverityRank(missedAlert.Severity), missedAlert.CreatedAt, null, GetTypeRank(missedAlert.Type)));
                    }
                    return;
                }

                var alert = new AlertResponse
                {
                    Id = $"meeting:{session.IDSesion}:{(isLive ? "live" : "soon")}",
                    DedupeKey = session.IDSesion.ToString(),
                    Type = isLive ? "meeting_live" : "meeting_soon",
                    Severity = isLive ? "critical" : "high",
                    Title = isLive
                                        ? $"Tu reunión '{request.Titulo}' ya empezó"
                                        : $"Tu reunión '{request.Titulo}' empieza en {FormatFutureDuration(startUtc - nowUtc)}",
                    Message = isLive ? "Entrá ahora para no perderte nada." : "Preparate y entrá a tiempo.",
                    CreatedAt = nowUtc,
                    ExpiresAt = isLive ? endUtc : startUtc.AddMinutes(5),
                    Sticky = isLive,
                    ChannelSuggested = isLive ? "modal" : "toast",
                    Cta = new AlertCtaResponse
                    {
                        Label = "Ir a la reunión",
                        Href = ctaHref
                    },
                    Data = new
                    {
                        requestId = request.IDSolicitudAyuda,
                        meetingId = session.IDSesion,
                        chatId,
                        startTime = startUtc,
                        endTime = endUtc,
                        helperId = reservation.IDUsuarioAyudante,
                        requesterId = request.IDUsuarioSolicitante,
                        role = string.Equals(reservation.IDUsuarioAyudante, normalizedUser, StringComparison.OrdinalIgnoreCase) ? "helper" : "requester"
                    }
                };

                alerts.Add((alert, GetSeverityRank(alert.Severity), alert.CreatedAt, startUtc, GetTypeRank(alert.Type)));
            }

            // Reuniones (para mí como solicitante)
            foreach (var request in myRequests)
            {
                if (request?.Disponibilidades is null) continue;

                foreach (var availability in request.Disponibilidades)
                {
                    if (availability?.Reservas is null) continue;

                    foreach (var reservation in availability.Reservas)
                        ProcessReservation(request, reservation);
                }
            }

            // Reuniones (para mí como ayudante)
            foreach (var reservation in helperReservations)
                ProcessReservation(reservation?.Disponibilidad?.Solicitud, reservation);

            // Solicitudes por vencer / sin franjas
            foreach (var request in myRequests)
            {
                if (request is null) continue;

                var state = request.SolicitudAyudaEstado?.Estado;
                if (!IsRequestActiveState(state)) continue;

                var expiresUtc = EnsureUtc(request.FechaVencimiento);
                if (expiresUtc <= nowUtc) continue;

                var relevantSlots = request.Disponibilidades?
                    .Where(s => s is not null && s.Active)
                    .ToList() ?? new List<SolicitudAyudaDisponibilidadModel>();

                var schedulingSlots = relevantSlots
                    .Where(s => s.Estado <= 2)
                    .ToList();

                var futureSlots = schedulingSlots
                    .Where(s => EnsureUtc(s.Fin) > nowUtc)
                    .ToList();

                var lastSlotEnd = schedulingSlots.Count > 0
                    ? EnsureUtc(schedulingSlots.Max(s => s.Fin))
                    : (DateTime?)null;

                var hasFutureSlots = futureSlots.Count > 0;
                var noSlots = !hasFutureSlots;

                var expiresSoon = expiresUtc <= nowUtc.AddHours(24);

                if (expiresSoon)
                {
                    var severity = noSlots ? "high" : "warning";
                    var expiringAlert = new AlertResponse
                    {
                        Id = $"request:{request.IDSolicitudAyuda}:expiring",
                        DedupeKey = $"{request.IDSolicitudAyuda}:expiring",
                        Type = "request_expiring",
                        Severity = severity,
                        Title = $"Tu solicitud '{request.Titulo}' vence pronto",
                        Message = noSlots
                                            ? $"Sin disponibilidad · la solicitud expira en {FormatFutureDuration(expiresUtc - nowUtc)}"
                                            : $"La solicitud expira en {FormatFutureDuration(expiresUtc - nowUtc)}",
                        CreatedAt = nowUtc,
                        ExpiresAt = expiresUtc,
                        Sticky = false,
                        ChannelSuggested = "toast",
                        Cta = new AlertCtaResponse
                        {
                            Label = "Ver solicitud",
                            Href = $"/forum/liveHelp/detail/{request.IDSolicitudAyuda}"
                        },
                        Data = new
                        {
                            requestId = request.IDSolicitudAyuda,
                            expiresAt = expiresUtc,
                            noSlots,
                            lastSlotEnd
                        }
                    };

                    alerts.Add((expiringAlert, GetSeverityRank(expiringAlert.Severity), expiringAlert.CreatedAt, null, GetTypeRank(expiringAlert.Type)));

                    if (noSlots) continue;
                }

                if (noSlots)
                {
                    var noSlotsAlert = new AlertResponse
                    {
                        Id = $"request:{request.IDSolicitudAyuda}:no_slots",
                        DedupeKey = $"{request.IDSolicitudAyuda}:no_slots",
                        Type = "no_slots",
                        Severity = "warning",
                        Title = "Sin disponibilidad",
                        Message = lastSlotEnd.HasValue
                                            ? $"La última franja venció hace {FormatPastDuration(nowUtc - lastSlotEnd.Value)}. Agregá una nueva."
                                            : "Tu solicitud no tiene franjas disponibles. Agregá una nueva.",
                        CreatedAt = lastSlotEnd ?? nowUtc,
                        Sticky = false,
                        ChannelSuggested = "banner",
                        Cta = new AlertCtaResponse
                        {
                            Label = "Agregar franjas",
                            Href = $"/forum/liveHelp/detail/{request.IDSolicitudAyuda}"
                        },
                        Data = new
                        {
                            requestId = request.IDSolicitudAyuda,
                            expiresAt = expiresUtc,
                            lastSlotEnd
                        }
                    };

                    alerts.Add((noSlotsAlert, GetSeverityRank(noSlotsAlert.Severity), noSlotsAlert.CreatedAt, null, GetTypeRank(noSlotsAlert.Type)));
                }
            }

            // Orden: severidad -> tipo -> (live/soon por MeetingStart) -> Created desc
            var ordered = alerts
                .OrderBy(a => a.Severity)
                .ThenBy(a => a.Type)
                .ThenBy(a => a.MeetingStart ?? DateTime.MaxValue)
                .ThenByDescending(a => a.SortCreatedAt)
                .Select(a => a.Alert)
                .ToList();

            // Un solo modal "critical"; el resto baja a toast
            var modalShown = false;
            foreach (var alert in ordered)
            {
                if (!string.Equals(alert.Severity, "critical", StringComparison.OrdinalIgnoreCase)) continue;
                if (!string.Equals(alert.ChannelSuggested, "modal", StringComparison.OrdinalIgnoreCase)) continue;

                if (modalShown)
                    alert.ChannelSuggested = "toast";
                else
                    modalShown = true;
            }

            return ordered;
        }


        public async Task<bool> MarkNotificationAsReadAsync(MarkNotificationAsReadRequest request)
        {
            try
            {
                var user = (await _repository.Get(x => x.Id == request.UserId, includeProperties: "UsersForum")).FirstOrDefault();

                if (user is null)
                    throw new ApiForumException("El usuario no existe.");
                else
                {
                    var repo = _unitOfWorkForum.GetRepository<INotificacionRepository>();
                    var notification = (await repo.Get(x => x.IDNotificacion == request.CodeNotification && x.IDUsuario == request.UserId)).FirstOrDefault();
                    if (notification is null)
                        return false;
                    else
                    {
                        notification.Leida = true;
                        await repo.Update(notification);
                        return await _unitOfWorkForum.SaveChangesAsync() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
