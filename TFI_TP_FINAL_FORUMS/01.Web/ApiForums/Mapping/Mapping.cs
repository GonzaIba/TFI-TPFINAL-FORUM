using AutoMapper;
using Core.Domain.Events;
using Core.Domain.GenericEntityClass;
using Core.Domain.IdentityModels;
using Core.Domain.Models;
using Core.Domain.Request;
using Core.Domain.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiForums.Mapping
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            // Mapping genérico para PaginatedList<T>
            CreateMap(typeof(PaginatedList<>), typeof(PaginatedList<>))
                // Indicas que la propiedad List se mapea de sí misma (AutoMapper infiere el tipo interno)
                .ForMember("List", opt => opt.MapFrom("List"));

            CreateMap(typeof(CursorPage<>), typeof(CursorPage<>))
                // Indicas que la propiedad List se mapea de sí misma (AutoMapper infiere el tipo interno)
                .ForMember("Items", opt => opt.MapFrom("Items"));


            #region Request
            CreateMap<CreatePublicationRequest, PublicacionModel>();
            #endregion

            #region Response
            CreateMap<PublicacionModel, PublicationResponse>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.FechaCreacion))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Titulo))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Contenido))
                .ForMember(dest => dest.Reward, opt => opt.MapFrom(src => src.Recompensa))
                .ForMember(dest => dest.Visits, opt => opt.MapFrom(src => src.Visitas))
                .ForMember(dest => dest.Answered, opt => opt.MapFrom(src => src.Respuestas.Any()))
                .ForMember(dest => dest.Closed, opt => opt.MapFrom(src => src.Cerrada))
                .ForMember(dest => dest.CodePublication, opt => opt.MapFrom(src => src.IDPublicacion))
                .ForMember(dest => dest.CodeUser, opt => opt.MapFrom(src => src.IDUsuario))
                .ForMember(dest => dest.UserCreator, opt =>
                    opt.MapFrom((src, dest, destMember, ctx) =>
                    ctx.Mapper.Map<UsersForumPreviewResponse>(src.Usuario)
                    )
                )
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.EtiquetasPublicacion.Select(x => x.Etiqueta.NombreEtiqueta)))
                .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Respuestas.Count))
                .ForMember(dest => dest.IsSaved, opt => opt.MapFrom((src, dest, destMember, context) =>
                    src.PublicacionesGuardadas?.Any(pg => pg.IDUsuario == (string)context.Items["UserId"]) == true
                ))
                .ReverseMap();

            CreateMap<EtiquetaModel, LabelResponse>()
                .ForMember(dest => dest.CodeLabel, opt => opt.MapFrom(src => src.IDEtiqueta))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.NombreEtiqueta))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.DescripcionEtiqueta))
                .ForMember(dest => dest.CountThisWeek, opt => opt.MapFrom(src => src.EtiquetasPublicaciones.Select(x => x.Publicacion).Where(y => y.FechaCreacion.ToUniversalTime() >= DateTime.UtcNow.AddDays(-7)).Count()))
                .ForMember(dest => dest.CountTotal, opt => opt.MapFrom(src => src.EtiquetasPublicaciones.Count()))
                .ReverseMap();

            CreateMap<TerminosCondicionesModel,TerminosCondicionesResponse>()
                .ForMember(dest => dest.Contenido, opt => opt.MapFrom(src => src.Descripcion))
                .ForMember(dest => dest.Titulo, opt => opt.MapFrom(src => src.Nombre))
                .ReverseMap();

            CreateMap<Users, UsersForumPreviewResponse>()
                .ForMember(dest => dest.CompleteName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName))
                .ForMember(dest => dest.Initials, opt => opt.MapFrom(src => ObtenerIniciales(src.FirstName + " " + src.LastName)))
                .ForMember(dest => dest.ShortDescription, opt => opt.MapFrom(src => src.UsersForum.ShortDescriptionForum))
                .ForMember(dest => dest.LongDescription, opt => opt.MapFrom(src => src.UsersForum.LongDescriptionForum))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.UsersForum.ImageForum))
                .ForMember(dest => dest.DateFrom, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.LastTimeOnline, opt => opt.MapFrom(src => src.UsersForum.LastTimeConnectedForum))
                .ForMember(dest => dest.Score, opt => opt.Ignore()) // Lo configuraremos después
                .ReverseMap();
            //.ForMember(dest => dest.UltimaVezConectado, opt => opt.MapFrom(src => src.UltimaVezConectadoForum)); // Asumo que Users tiene una propiedad llamada LastConnected

            CreateMap<Users, UserForumResponse>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate));
            //.ForMember(dest => dest.Puntaje, opt => opt.MapFrom(src => src.RecompensasUsuarios.Sum(x => x.CantidadRecompensa)));

            CreateMap<Users, DetailsUserForumResponse>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.LanguagePreference, opt => opt.MapFrom(src => src.LanguagePreference))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.ShortDescriptionForum, opt => opt.MapFrom(src => src.UsersForum.ShortDescriptionForum))
                .ForMember(dest => dest.LongDescriptionForum, opt => opt.MapFrom(src => src.UsersForum.LongDescriptionForum))
                .ForMember(dest => dest.ImageForum, opt => opt.MapFrom(src => src.UsersForum.ImageForum))
                .ForMember(dest => dest.LastTimeConnectedForum, opt => opt.MapFrom(src => src.UsersForum.LastTimeConnectedForum))
                .ForMember(dest => dest.QuantityResponses, opt => opt.MapFrom(src => src.Respuestas.Count()))
                .ForMember(dest => dest.NumberPostsCreated, opt => opt.MapFrom(src => src.Publicaciones.Count()))
                .ForMember(dest => dest.Medals, opt => opt.MapFrom(src => src.UsuarioMedallas.Select(x => new Medalla { NameMedal = x.Medalla.NombreMedalla, DateObtained = x.FechaObtenido, Description = x.Medalla.Descripcion, ImageMedal = x.Medalla.ImagenMedalla })));
            //.ForMember(dest => dest.Puntaje, opt => opt.MapFrom(src => src.RecompensasUsuarios.Sum(x => x.CantidadRecompensa)))

            CreateMap<PublicacionModel, PublicationDetailResponse>()
                .ForMember(dest => dest.CodePublication, opt => opt.MapFrom(src => src.IDPublicacion))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.Usuario))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Titulo))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Contenido))
                .ForMember(dest => dest.Reward, opt => opt.MapFrom(src => src.Recompensa))
                .ForMember(dest => dest.Visits, opt => opt.MapFrom(src => src.Visitas))
                .ForMember(dest => dest.Votes, opt => opt.MapFrom(src => ContadorVotosPublicaciones(src.PublicacionesVotos)))
                .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Respuestas))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.FechaCreacion))
                .ForMember(dest => dest.ClosedDate, opt => opt.MapFrom(src => src.FechaCierre))
                .ForMember(dest => dest.VotedPositive, opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    var userId = context.Items["UserId"] as string;
                    if (string.IsNullOrEmpty(userId))
                        return (bool?)null;
                    var voto = src.PublicacionesVotos?.FirstOrDefault(v => v.IDUsuario == userId);
                    return voto?.Positivo;
                }))
                .ForMember(dest => dest.IsAuthor, opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    var userId = context.Items["UserId"] as string;
                    if (string.IsNullOrEmpty(userId))
                        return (bool?)null;

                    return src.IDUsuario == userId;
                }));
            ///............

            CreateMap<RespuestaModel, AnswerResponse>()
                .ForMember(dest => dest.CodeAnswer, opt => opt.MapFrom(src => src.IDRespuesta))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.Usuario))
                .ForMember(dest => dest.TextResponse, opt => opt.MapFrom(src => src.TextoRespuesta))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.FechaCreacion))
                .ForMember(dest => dest.CorrectAnswer, opt => opt.MapFrom(src => src.RespuestaCorrecta))
                .ForMember(dest => dest.Votes, opt => opt.MapFrom(src => ContadorVotos(src.RespuestasVotos)))
                //////files...
                .ForMember(dest => dest.VotedPositive, opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    var userId = context.Items["UserId"] as string;
                    if (string.IsNullOrEmpty(userId))
                        return (bool?)null;
                    var voto = src.RespuestasVotos?.FirstOrDefault(v => v.IDUsuario == userId);
                    return voto?.Positivo;
                }))
                .ForMember(dest => dest.IsAuthor, opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    var userId = context.Items["UserId"] as string;
                    if (string.IsNullOrEmpty(userId))
                        return (bool?)null;
                    return src.IDUsuario == userId;
                }));

            CreateMap<ArchivoModel, FilesResponse>()
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.NombreArchivo))
                .ForMember(dest => dest.TypeFile, opt => opt.MapFrom(src => src.TipoArchivo))
                .ForMember(dest => dest.File, opt => opt.MapFrom(src => src.Archivo));

            CreateMap<PublicationDetailResponse, UsersForumPreviewResponse>();
            CreateMap<UsersForumPreviewResponse, PublicationDetailResponse>();

            CreateMap<NotificacionesModel, NotificationsResponse>()
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Mensaje))
                .ForMember(dest => dest.Readed, opt => opt.MapFrom(src => src.Leida))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.FechaNotificacion))
                .ForMember(dest => dest.CodeNotification, opt => opt.MapFrom(src => src.IDNotificacion));

            CreateMap<SolicitudAyudaModel, RequestHelpResponse>()
                .ForMember(dest => dest.CodeRequestHelp, opt => opt.MapFrom(src => src.IDSolicitudAyuda))
                .ForMember(dest => dest.TitleHelp, opt => opt.MapFrom(src => src.Titulo))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Descripcion))
                .ForMember(dest => dest.Labels, opt => opt.MapFrom(src => src.SolicitudAyudaEtiquetas.Select(x => x.Etiqueta.NombreEtiqueta)))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreateDate))
                .ForMember(dest => dest.Regard, opt => opt.MapFrom(src => src.RecompensaBase))
                .ForMember(dest => dest.ExpiresAt, opt => opt.MapFrom(src => src.FechaVencimiento))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.SolicitudAyudaEstado.Estado))
                .ForMember(d => d.Languages, o => o.MapFrom(s =>
                    string.IsNullOrWhiteSpace(s.Lenguaje)
                    ? new List<string>()
                    : s.Lenguaje
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .Where(x => x.Length > 0)
                        .ToList()
                ))
                .ForMember(dest => dest.TimeSlot, opt => opt.MapFrom(src => BuildTimeSlots(src)))
                .ForMember(dest => dest.UserCreator, opt =>
                    opt.MapFrom((src, dest, destMember, ctx) =>
                    ctx.Mapper.Map<UsersForumPreviewResponse>(src.UsuarioSolicitante))
                )
                .ReverseMap();

            // Chat mappings

            CreateMap<SolicitudAyudaChatModel, HelpRequestChatsResponse>()
                .ForMember(d => d.ChatId, o => o.MapFrom(s => s.IDChat))
                .ForMember(d => d.RequestId, o => o.MapFrom(s => s.IDSolicitudAyuda))
                .ForMember(d => d.State, o => o.MapFrom(s => "Abierto")) // mapear desde EstadoChat si lo tenés
                .ForMember(d => d.CreatedAt, o => o.MapFrom(s => s.CreateDate))
                .ForMember(d => d.Active, o => o.MapFrom(s => s.Active))
                // other (el ayudante)
                .ForMember(dest => dest.Other, opt =>
                    opt.MapFrom((src, dest, destMember, ctx) =>
                    ctx.Mapper.Map<UsersForumPreviewResponse>(src.UsuarioAyudante))
                )
                // lastMessage (preview, fecha, si es mío)
                .ForMember(d => d.LastMessage, o => o.MapFrom((s, _, __, ctx) =>
                {
                    var userId = ctx.Items.TryGetValue("UserId", out var u) ? u as string : null;
                    var last = s.Mensajes
                                .OrderByDescending(m => m.CreateDate)
                                .ThenByDescending(m => m.IDMensaje)
                                .FirstOrDefault();

                    if (last == null) return null;

                    var preview = last.Mensaje?.Length > 180
                                    ? last.Mensaje.Substring(0, 180) + "…"
                                    : last.Mensaje ?? string.Empty;

                    return new HelpRequestChatsResponse.LastMessageView
                    {
                        Preview = preview,
                        At = last.CreateDate,
                        FromMe = !string.IsNullOrWhiteSpace(userId) &&
                                    string.Equals(last.IDUsuario, userId, StringComparison.OrdinalIgnoreCase)
                    };
                }))
                // unreadCount provisto (query directa en servicio)
                .ForMember(d => d.UnreadCount, o => o.MapFrom((s, _, __, ctx) =>
                {
                    if (ctx.Items.TryGetValue("UnreadByChat", out var raw) &&
                        raw is IDictionary<int, int> map &&
                        map.TryGetValue(s.IDChat, out var count))
                    {
                        return count;
                    }

                    return 0;
                }));

            // Detail: from SolicitudAyudaChatModel -> HelpRequestChatDetailResponse
            CreateMap<SolicitudAyudaChatModel, HelpRequestChatDetailResponse>()
                .ForMember(d => d.ChatCode, o => o.MapFrom(s => s.IDChat))
                .ForMember(d => d.RequestCode, o => o.MapFrom(s => s.IDSolicitudAyuda))
                .ForMember(d => d.State, o => o.MapFrom(s => "Abierto")) // TODO: mapear desde Estado si corresponde
                .ForMember(d => d.CreatedAt, o => o.MapFrom(s => s.CreateDate))
                .ForMember(d => d.Active, o => o.MapFrom(s => s.Active))
                // Other (el otro participante respecto del usuario actual)
                .ForMember(d => d.Other, o => o.MapFrom((s, _, __, ctx) =>
                {
                    // Busco primero “el que no soy yo”; si no hay, tomo ayudante por Rol=1 como fallback
                    var otherPart = s.UsuarioAyudante;
                    return otherPart != null
                        ? ctx.Mapper.Map<UsersForumPreviewResponse>(otherPart)
                        : null;
                }))
                // UnreadCount pre-calculado (query COUNT en servicio)
                .ForMember(d => d.UnreadCount, o => o.MapFrom((_, _, __, ctx) =>
                {
                    return ctx.Items.TryGetValue("UnreadCount", out var raw) && raw is int count
                        ? count
                        : 0;
                }))
                // Messages (orden cronológico ascendente)
                .ForMember(d => d.Messages, o => o.MapFrom((s, _, __, ctx) =>
                {
                    var me = ctx.Items.TryGetValue("UserId", out var u) ? u as string : null;

                    // Determino el "otro" (para read receipts de mis mensajes)
                    var otherId = s.Participantes?
                        .FirstOrDefault(p => !string.Equals(p.IDUsuario, me, StringComparison.OrdinalIgnoreCase))?
                        .IDUsuario;

                    var ordered = s.Mensajes?
                        .OrderBy(m => m.CreateDate)
                        .ThenBy(m => m.IDMensaje)
                        .Select(m =>
                        {
                            var fromMe = !string.IsNullOrWhiteSpace(me) &&
                                         string.Equals(m.IDUsuario, me, StringComparison.OrdinalIgnoreCase);

                            var isRead = false;
                            if (fromMe)
                            {
                                if (!string.IsNullOrWhiteSpace(otherId))
                                {
                                    isRead = m.Lecturas?.Any(l =>
                                        string.Equals(l.IDUsuario, otherId, StringComparison.OrdinalIgnoreCase)) ?? false;
                                }
                            }
                            else if (!string.IsNullOrWhiteSpace(me))
                            {
                                isRead = m.Lecturas?.Any(l =>
                                    string.Equals(l.IDUsuario, me, StringComparison.OrdinalIgnoreCase)) ?? false;
                            }

                            return new HelpRequestChatDetailResponse.MessageView
                            {
                                CodeMessage = m.IDMensaje,
                                Text = m.Mensaje ?? string.Empty,
                                At = m.CreateDate,
                                FromMe = fromMe,
                                IsRead = isRead
                            };
                        })
                        .ToList() ?? new List<HelpRequestChatDetailResponse.MessageView>();

                    return ordered;
                }));

            // Message: from SolicitudAyudaChatMensajeModel -> ChatMessageResponse
            CreateMap<SolicitudAyudaChatMensajeModel, ChatMessageResponse>()
                .ForMember(d => d.CodeMessage, o => o.MapFrom(s => s.IDMensaje))
                .ForMember(d => d.CodeChat, o => o.MapFrom(s => s.IDChat))
                .ForMember(d => d.Message, o => o.MapFrom(s => s.Mensaje ?? string.Empty))
                .ForMember(d => d.CreatedAt, o => o.MapFrom(s => s.CreateDate))
                // Readed: si el backend seteó un flag rápido, úsalo; si no, calculá por lecturas del usuario actual
                .ForMember(d => d.Readed, o => o.MapFrom((s, _, __, ctx) =>
                {
                    if (s.LeidoPorUsuarioActual) return s.LeidoPorUsuarioActual;

                    var me = ctx.Items.TryGetValue("UserId", out var u) ? u as string : null;
                    if (string.IsNullOrWhiteSpace(me)) return false;

                    return s.Lecturas?.Any(l =>
                        string.Equals(l.IDUsuario, me, StringComparison.OrdinalIgnoreCase)) ?? false;
                }))
                // SentByMe: útil para listados/históricos; en tu acción luego lo podés sobrescribir si querés
                .ForMember(d => d.SentByMe, o => o.MapFrom((s, _, __, ctx) =>
                {
                    var me = ctx.Items.TryGetValue("UserId", out var u) ? u as string : null;
                    return !string.IsNullOrWhiteSpace(me) &&
                           string.Equals(s.IDUsuario, me, StringComparison.OrdinalIgnoreCase);
                }));

            CreateMap<SolicitudAyudaModel, RequestHelpConfirmedResponse>()
                .ForMember(d => d.CodeRequestHelp, o => o.MapFrom(s => s.IDSolicitudAyuda))
                .ForMember(d => d.TitleHelp, o => o.MapFrom(s => s.Titulo))
                .ForMember(d => d.Message, o => o.MapFrom(s => s.Descripcion))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.SolicitudAyudaEstado.Estado))
                .ForMember(d => d.Languages, o => o.MapFrom(s =>
                    string.IsNullOrWhiteSpace(s.Lenguaje)
                    ? new List<string>()
                    : s.Lenguaje
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .Where(x => x.Length > 0)
                        .ToList()
                ))
                .ForMember(d => d.Labels, o => o.MapFrom(s => s.SolicitudAyudaEtiquetas.Select(x => x.Etiqueta.NombreEtiqueta)))
                .ForMember(d => d.CreatedAt, o => o.MapFrom(s => s.CreateDate))
                .ForMember(d => d.Regard, o => o.MapFrom(s => s.RecompensaBase))
                .ForMember(d => d.InitAt, o => o.MapFrom(s =>
                    s.Disponibilidades
                     .Where(disp => disp.Reservas.Any())
                     .OrderBy(disp => disp.Inicio)
                     .Select(disp => (DateTime?)disp.Inicio)
                     .FirstOrDefault()
                ))
                .ForMember(d => d.IsOwner, o => o.MapFrom((s, _, __, ctx) =>
                {
                    var userId = ctx.Items.TryGetValue("UserId", out var u) ? u as string : null;
                    if (string.IsNullOrEmpty(userId))
                        return false;
                    return s.IDUsuarioSolicitante == userId;
                }))
                .ForMember(d => d.UserCreator, o =>
                    o.MapFrom((s, d, dm, ctx) =>
                    ctx.Mapper.Map<UsersForumPreviewResponse>(s.UsuarioSolicitante))
                )
                .ReverseMap();

            CreateMap<SesionAyudaModel,GetSessionResponse>()
                .ForMember(d => d.CodeSession, o => o.MapFrom(s => s.IDSesion))
                .ForMember(d => d.Domain, o => o.MapFrom(s => s.Dominio))
                .ForMember(d => d.RoomName, o => o.MapFrom(s => s.NombreSala))
                .ForMember(d => d.InitAt, o => o.MapFrom(s => s.Reserva.Disponibilidad.Inicio))
                .ForMember(d => d.ExpiresAt, o => o.MapFrom(s => s.Reserva.Disponibilidad.Fin))
                .ForMember(d => d.IsOwner, o => o.MapFrom((s, _, __, ctx) =>
                {
                    var userId = ctx.Items.TryGetValue("UserId", out var u) ? u as string : null;
                    if (string.IsNullOrEmpty(userId))
                        return false;
                    return s.Reserva.Disponibilidad.Solicitud.IDUsuarioSolicitante == userId;
                }))
                .ReverseMap();
            #endregion

            CreateMap<AnswerResponse, AddAnswerEvent>();
        }

        #region Helpers
        public int ContadorVotosPublicaciones(ICollection<PublicacionVotoModel> votos)
        {
            int contador = 0;
            foreach (var voto in votos)
            {
                if (voto.Positivo)
                    contador++;
                else
                    contador--;
            }
            return contador;
        }

        public int ContadorVotos(ICollection<RespuestaVotoModel> votos)
        {
            int contador = 0;
            foreach (var voto in votos)
            {
                if (voto.Positivo)
                    contador++;
                else
                    contador--;
            }
            return contador;
        }

        public static string ObtenerIniciales(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return string.Empty;

            var palabras = nombre.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var iniciales = new StringBuilder();

            foreach (var palabra in palabras)
            {
                iniciales.Append(char.ToUpper(palabra[0]));
            }

            return iniciales.ToString();
        }

        private static RequestHelpTimeSlot BuildTimeSlots(SolicitudAyudaModel src)
        {
            var now = DateTime.UtcNow;
            var slotItems = new List<TimeSlotItem>();
            if (src?.Disponibilidades == null) return new RequestHelpTimeSlot { Slots = slotItems };
            foreach (var disp in src.Disponibilidades)
            {
                if (disp.Estado == 1 && disp.Fin > now)
                {
                    var start = disp.Inicio > now ? disp.Inicio : now;
                    var end = disp.Fin;
                    var interval = (end - start).TotalMinutes >= 30 ? TimeSpan.FromMinutes(30) : TimeSpan.FromMinutes(15);
                    for (var dt = start; dt.Add(interval) <= end; dt = dt.Add(interval))
                    {
                        var slotEnd = dt.Add(interval) <= end ? dt.Add(interval) : end;
                        slotItems.Add(new TimeSlotItem { CodeSlot = disp.IDDisponibilidad, Start = dt, End = slotEnd });
                    }
                }
            }
            return new RequestHelpTimeSlot { Slots = slotItems };
        }

        // helper local para iniciales (puede vivir donde prefieras)
        private static string BuildInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "--";
            var parts = name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpperInvariant();
            return (parts[0][0].ToString() + parts[^1][0].ToString()).ToUpperInvariant();
        }
        #endregion
    }
}


