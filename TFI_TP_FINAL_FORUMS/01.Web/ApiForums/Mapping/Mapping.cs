using AutoMapper;
using Core.Domain.DTOs;
using Core.Domain.IdentityModels;
using Core.Domain.Models;
using Core.Domain.Request;
using Core.Domain.Response;
using CrossCutting.Helpers.ResultClasses;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace ApiForums.Mapping
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            #region Request
            CreateMap<CreatePublicationRequest, PublicacionModel>();
            #endregion

            #region Response
            CreateMap<EtiquetaPublicacionModel, LabelsPublicationsResponse>();
            CreateMap<LabelResponse, LabelsPublicationsResponse>();

            CreateMap<PublicacionModel, PublicationResponse>()
                .ForMember(dest => dest.CodigoPublicacion, opt => opt.MapFrom(src => src.IDPublicacion))
                .ForMember(dest => dest.CodigoUsuario, opt => opt.MapFrom(src => src.IDUsuario))
                .ForMember(dest => dest.Etiquetas, opt => opt.MapFrom(src => src.EtiquetasPublicacion.Select(x=>x.Etiqueta.NombreEtiqueta)))
                .ForMember(dest => dest.Respuestas, opt => opt.MapFrom(src => src.Respuestas.Count))
                .ReverseMap();

            CreateMap<EtiquetaModel, LabelResponse>()
                .ForMember(dest => dest.CodigoEtiqueta, opt => opt.MapFrom(src => src.IDEtiqueta))
                .ReverseMap()
                .ForMember(dest => dest.IDEtiqueta, opt => opt.MapFrom(src => src.CodigoEtiqueta));

            CreateMap<Users, UsersForumPreviewResponse>()
                .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => src.Nombre + " " + src.Apellido)) // Asumo que Users tiene una propiedad llamada Nombre
                .ForMember(dest => dest.DescripcionCorta, opt => opt.MapFrom(src => src.UsersForum.ShortDescriptionForum))
                .ForMember(dest => dest.DescripcionLarga, opt => opt.MapFrom(src => src.UsersForum.LongDescriptionForum))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.UsersForum.ImageForum))
                .ForMember(dest => dest.Puntaje, opt => opt.Ignore()); // Lo configuraremos después
                //.ForMember(dest => dest.UltimaVezConectado, opt => opt.MapFrom(src => src.UltimaVezConectadoForum)); // Asumo que Users tiene una propiedad llamada LastConnected

            CreateMap<Users, UsersForumResponse>()
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FechaCreado, opt => opt.MapFrom(src => src.FechaCreado))
                .ForMember(dest => dest.Puntaje, opt => opt.MapFrom(src => src.RecompensasUsuarios.Sum(x => x.CantidadRecompensa)));

            CreateMap<Users, DetailsUserForumResponse>()
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Apellido, opt => opt.MapFrom(src => src.Apellido))
                .ForMember(dest => dest.LenguajePreferencia, opt => opt.MapFrom(src => src.LenguajePreferencia))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FechaCreado, opt => opt.MapFrom(src => src.FechaCreado))
                .ForMember(dest => dest.ShortDescriptionForum, opt => opt.MapFrom(src => src.UsersForum.ShortDescriptionForum))
                .ForMember(dest => dest.LongDescriptionForum, opt => opt.MapFrom(src => src.UsersForum.LongDescriptionForum))
                .ForMember(dest => dest.ImageForum, opt => opt.MapFrom(src => src.UsersForum.ImageForum))
                .ForMember(dest => dest.LastTimeConnectedForum, opt => opt.MapFrom(src => src.UsersForum.LastTimeConnectedForum))
                .ForMember(dest => dest.Puntaje, opt => opt.MapFrom(src => src.RecompensasUsuarios.Sum(x => x.CantidadRecompensa)))
                .ForMember(dest => dest.CantidadRespuestas, opt => opt.MapFrom(src => src.Respuestas.Count()))
                .ForMember(dest => dest.CantidadPublicacionesCreadas, opt => opt.MapFrom(src => src.Publicaciones.Count()))
                .ForMember(dest => dest.Medallas, opt => opt.MapFrom(src => src.UsuarioMedallas.Select(x => new Medalla { NombreMedalla = x.Medalla.NombreMedalla, FechaObtenido = x.FechaObtenido, Descripcion = x.Medalla.Descripcion, ImagenMedalla = x.Medalla.ImagenMedalla })));

            CreateMap<PublicacionModel, PublicationDetailResponse>()
                .ForMember(dest => dest.CodigoPublicacion, opt => opt.MapFrom(src => src.IDPublicacion))
                //.ForMember(dest => dest.UsuarioCreador, opt => opt.MapFrom(src => src.IDUsuario))
                .ForMember(dest => dest.Titulo, opt => opt.MapFrom(src => src.Titulo))
                .ForMember(dest => dest.Contenido, opt => opt.MapFrom(src => src.Contenido))
                .ForMember(dest => dest.Recompensa, opt => opt.MapFrom(src => src.Recompensa))
                .ForMember(dest => dest.Visitas, opt => opt.MapFrom(src => src.Visitas))
                .ForMember(dest => dest.Votos, opt => opt.MapFrom(src => ContadorVotosPublicaciones(src.PublicacionesVotos)))
                .ForMember(dest => dest.VotadoPositivo, opt => opt.MapFrom(src => src.PublicacionesVotos.Any(x => x.IDUsuario == src.IDUsuario)))
                .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => src.FechaCreacion))
                .ForMember(dest => dest.FechaCierre, opt => opt.MapFrom(src => src.FechaCierre));

            CreateMap<PublicacionModel, PublicationDetailResponse>()
                .ForMember(dest => dest.CodigoPublicacion, opt => opt.MapFrom(src => src.IDPublicacion))
                //.ForMember(dest => dest.UsuarioCreador, opt => opt.MapFrom(src => src.IDUsuario))
                .ForMember(dest => dest.Titulo, opt => opt.MapFrom(src => src.Titulo))
                .ForMember(dest => dest.Contenido, opt => opt.MapFrom(src => src.Contenido))
                .ForMember(dest => dest.Recompensa, opt => opt.MapFrom(src => src.Recompensa))
                .ForMember(dest => dest.Visitas, opt => opt.MapFrom(src => src.Visitas))
                .ForMember(dest => dest.Votos, opt => opt.MapFrom(src => ContadorVotosPublicaciones(src.PublicacionesVotos)))
                .ForMember(dest => dest.VotadoPositivo, opt => opt.MapFrom(src => src.PublicacionesVotos.Any(x => x.IDUsuario == src.IDUsuario)))
                .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => src.FechaCreacion))
                .ForMember(dest => dest.FechaCierre, opt => opt.MapFrom(src => src.FechaCierre));

            CreateMap<RespuestaModel, AnswerResponse>()
                .ForMember(dest => dest.CodigoRespuesta, opt => opt.MapFrom(src => src.IDRespuesta))
                .ForMember(dest => dest.TextoRespuesta, opt => opt.MapFrom(src => src.TextoRespuesta))
                .ForMember(dest => dest.RespuestaCorrecta, opt => opt.MapFrom(src => src.RespuestaCorrecta))
                .ForMember(dest => dest.Votos, opt => opt.MapFrom(src => ContadorVotosPublicaciones(src.RespuestasVotos)))
                .ForMember(dest => dest.VotadoPositivo, opt => opt.MapFrom(src => src.RespuestasVotos.Any(x => x.IDUsuario == src.IDUsuario)))
                .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => src.FechaCreacion));

            CreateMap<ArchivoModel, FilesResponse>()
                .ForMember(dest => dest.NombreArchivo, opt => opt.MapFrom(src => src.NombreArchivo))
                .ForMember(dest => dest.TipoArchivo, opt => opt.MapFrom(src => src.TipoArchivo))
                .ForMember(dest => dest.Archivo, opt => opt.MapFrom(src => src.Archivo));

            CreateMap<PublicationDetailResponse, UsersForumPreviewResponse>();
            CreateMap<UsersForumPreviewResponse, PublicationDetailResponse>();
            #endregion
        }

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

        public int ContadorVotosPublicaciones(ICollection<RespuestaVotoModel> votos)
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
    }
}
