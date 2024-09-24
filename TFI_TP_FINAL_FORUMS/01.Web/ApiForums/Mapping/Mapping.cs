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
            CreateMap<CrearPublicacionRequest, PublicacionModel>();
            #endregion

            #region Response
            CreateMap<EtiquetaPublicacionModel, EtiquetasPublicacionesResponse>();
            CreateMap<EtiquetasResponse, EtiquetasPublicacionesResponse>();

            CreateMap<PublicacionModel, PublicacionesResponse>()
                .ForMember(dest => dest.NumeroPublicacion, opt => opt.MapFrom(src => src.IDPublicacion))
                .ForMember(dest => dest.CodigoUsuario, opt => opt.MapFrom(src => src.IDUsuario))
                .ForMember(dest => dest.Etiquetas, opt => opt.MapFrom(src => src.EtiquetasPublicacion.Select(x=>x.Etiqueta.NombreEtiqueta)))
                .ForMember(dest => dest.Respuestas, opt => opt.MapFrom(src => src.Respuestas.Count))
                .ReverseMap();

            CreateMap<EtiquetaModel, EtiquetasResponse>()
                .ForMember(dest => dest.CodigoEtiqueta, opt => opt.MapFrom(src => src.IDEtiqueta))
                .ReverseMap()
                .ForMember(dest => dest.IDEtiqueta, opt => opt.MapFrom(src => src.CodigoEtiqueta));

            CreateMap<Users, UsersTopResponse>()
                .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => src.Nombre)) // Asumo que Users tiene una propiedad llamada Nombre
                //.ForMember(dest => dest.DescripcionCorta, opt => opt.MapFrom(src => src.DescripcionCortaForum))
                //.ForMember(dest => dest.DescripcionLarga, opt => opt.MapFrom(src => src.DescripcionLargaForum))
                //.ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.ImageForum))
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
            #endregion
        }
    }
}
