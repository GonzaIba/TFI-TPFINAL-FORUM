using AutoMapper;
using Core.Domain.DTOs;
using Core.Domain.IdentityModels;
using Core.Domain.Models;
using Core.Domain.Request;
using Core.Domain.Response;
using CrossCutting.Helpers.ResultClasses;
using Microsoft.AspNetCore.Identity;

namespace ApiForums.Mapping
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CrearPublicacionRequest, PublicacionModel>();

            CreateMap<PublicacionModel, PublicacionesResponse>()
            .ForMember(dest => dest.NumeroPublicacion, opt => opt.MapFrom(src => src.IDPublicacion))
            .ForMember(dest => dest.CodigoUsuario, opt => opt.MapFrom(src => src.IDUsuario))
            .ReverseMap();
        }
    }
}
