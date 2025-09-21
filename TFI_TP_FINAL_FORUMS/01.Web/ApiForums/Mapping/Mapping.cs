using AutoMapper;
using Core.Domain.Events;
using Core.Domain.GenericEntityClass;
using Core.Domain.IdentityModels;
using Core.Domain.Models;
using Core.Domain.Request;
using Core.Domain.Response;
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
                .ForMember(dest => dest.CountThisWeek, opt => opt.MapFrom(src => src.EtiquetasPublicaciones.Select(x=> x.Publicacion).Where(y=> y.FechaCreacion.ToUniversalTime() >= DateTime.UtcNow.AddDays(-7)).Count()))
                .ForMember(dest => dest.CountTotal, opt => opt.MapFrom(src => src.EtiquetasPublicaciones.Count()))
                .ReverseMap();

            CreateMap<Users, UsersForumPreviewResponse>()
                .ForMember(dest => dest.CompleteName, opt => opt.MapFrom(src => src.Nombre + " " + src.Apellido))
                .ForMember(dest => dest.Initials, opt => opt.MapFrom(src => ObtenerIniciales(src.Nombre + " " + src.Apellido)))
                .ForMember(dest => dest.ShortDescription, opt => opt.MapFrom(src => src.UsersForum.ShortDescriptionForum))
                .ForMember(dest => dest.LongDescription, opt => opt.MapFrom(src => src.UsersForum.LongDescriptionForum))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.UsersForum.ImageForum))
                .ForMember(dest => dest.DateFrom, opt => opt.MapFrom(src => src.FechaCreado))
                .ForMember(dest => dest.LastTimeOnline, opt => opt.MapFrom(src => src.UsersForum.LastTimeConnectedForum))
                .ForMember(dest => dest.Score, opt => opt.Ignore()) // Lo configuraremos después
                .ReverseMap();
                //.ForMember(dest => dest.UltimaVezConectado, opt => opt.MapFrom(src => src.UltimaVezConectadoForum)); // Asumo que Users tiene una propiedad llamada LastConnected

            CreateMap<Users, UserForumResponse>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.FechaCreado));
                //.ForMember(dest => dest.Puntaje, opt => opt.MapFrom(src => src.RecompensasUsuarios.Sum(x => x.CantidadRecompensa)));

            CreateMap<Users, DetailsUserForumResponse>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Apellido))
                .ForMember(dest => dest.LanguagePreference, opt => opt.MapFrom(src => src.LenguajePreferencia))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.FechaCreado))
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
                .ForMember(dest => dest.CodeUser, opt => opt.MapFrom(src => src.IDUsuario))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.FechaNotificacion))
                .ForMember(dest => dest.CodeNotification, opt => opt.MapFrom(src => src.IDNotificacion));

            CreateMap<SolicitudAyudaModel, RequestHelpResponse>()
                .ForMember(dest => dest.TitleHelp, opt => opt.MapFrom(src => src.Titulo))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Descripcion))
                .ForMember(dest => dest.Labels, opt => opt.MapFrom(src => src.SolicitudAyudaEtiquetas.Select(x => x.Etiqueta.NombreEtiqueta)))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreateDate))
                .ForMember(dest => dest.Regard, opt => opt.MapFrom(src => src.RecompensaBase))
                .ForMember(dest => dest.ExpiresAt, opt => opt.MapFrom(src => src.CreateDate.AddHours(48)))
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
                .ForMember(dest => dest.UserCreator, opt =>
                    opt.MapFrom((src, dest, destMember, ctx) =>
                    ctx.Mapper.Map<UsersForumPreviewResponse>(src.UsuarioSolicitante))
                )
                .ReverseMap();
            #endregion

            #region Events
            CreateMap<AddAnswerEvent, AnswerResponse>().ReverseMap();
            #endregion
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
        #endregion
    }
}
