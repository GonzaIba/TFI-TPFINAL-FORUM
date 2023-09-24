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
            #endregion
        }
    }
}
