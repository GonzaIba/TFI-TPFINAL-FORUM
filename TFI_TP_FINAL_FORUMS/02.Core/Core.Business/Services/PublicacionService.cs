using AutoMapper;
using Core.Contracts.Repositories;
using Core.Contracts.Services;
using Core.Domain.Exceptions;
using Core.Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Business.Services
{
    public class PublicacionService : GenericService<PublicacionModel>, IPublicacionService
    {
        private readonly IUsersService _usersService;
        public PublicacionService(
            IUnitOfWork unitOfWork,
            IUsersService usersService
            )
        : base(unitOfWork, unitOfWork.GetRepository<IPublicacionRepository>())
        {
            _usersService = usersService;
        }

        public async Task<bool> CrearPublicacion(string userId, PublicacionModel publicacion)
        {
            try
            {
                var user = await _usersService.GetByIdAsync(userId);
                if (user == null)
                    throw new ApiForumException("No existe el usuario.");

                publicacion.IDUsuario = user.Id;
                publicacion.CreateDate = DateTime.Now;
                publicacion.FechaCreacion = DateTime.Now;
                publicacion.FechaCierre = null;
                await _repository.Insert(publicacion);
                if (await _unitOfWork.Complete())
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<PublicacionModel>> ObtenerPublicaciones()
        {
            try
            {
                var result = await _repository.Get(tracking: false, ignoreQueryFilters: true);
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
