using Core.Contracts.Repositories;
using Core.Contracts.Services;
using Core.Domain.IdentityModels;
using CrossCutting.Extensions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Business.Services
{
    public class RolesService : GenericService<Roles>, IRolesService
    {
        private readonly RoleManager<Roles> _roleManager;
        public RolesService(IUnitOfWork unitOfWork, RoleManager<Roles> roleManager)
            : base(unitOfWork, unitOfWork.GetRepository<IRolesRepository>())
        {
            _roleManager = roleManager;
        }

        public async Task<bool> CreateRoleAsync(Roles roles)
        {
            var result = await _roleManager.CreateAsync(roles);
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.ToString("\n"));
            }
            return true;
        }

        public async Task<bool> DeleteRoleAsync(string id)
        {
            try
            {
                var Rol = (await _repository.Get(x => x.Id == id, tracking: false)).FirstOrDefault(); //Esta manera y la de abajo son válidas...
                if (Rol == null)
                {
                    return false;
                }
                //var Rol = _roleManager.GetRoleNameAsync(new Privileges { Id = id });
                var result = await _roleManager.DeleteAsync(Rol);
                //var result = await _roleManager.DeleteAsync(new Privileges { Id = id });
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.ToString("\n"));
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }         
        }

        public async Task<Roles> UpdateRoleAsync(Roles roles)
        {
            var role = (await _repository.Get(x => x.Id == roles.Id, tracking:false)).FirstOrDefault();
            if(role.NormalizedName == "ADMINISTRADOR" || role.NormalizedName == "USER")
                throw new Exception($"No se puede eliminar/editar el rol {role.Name} porque es un rol base del sistema.");

            var result = await _roleManager.UpdateAsync(roles);
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.ToString("\n"));
            }
            return roles;
        }

        public async Task<List<Roles>> GetRolesAsync()
        {
            return (await _repository.Get()).ToList();
        }

        public async Task<Roles> GetRoleByIdAsync(string id)
        {
            return await _roleManager.FindByIdAsync(id);
        }
    }
}
