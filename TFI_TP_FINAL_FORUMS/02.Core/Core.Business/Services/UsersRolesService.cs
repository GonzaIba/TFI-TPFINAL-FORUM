using Core.Contracts.Repositories;
using Core.Contracts.Services;
using Core.Domain.IdentityModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Business.Services
{
    public class UsersRolesService : GenericService<UsersRoles>, IUsersRolesService
    {
        private readonly UserManager<Users> _userManager;
        private readonly RoleManager<Roles> _roleManager;
        public UsersRolesService(IUnitOfWork unitOfWork, UserManager<Users> userManager, RoleManager<Roles> roleManager) 
            : base(unitOfWork, unitOfWork.GetRepository<IUsersRolesRepository>())
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }       
        
        public async Task<bool> AssignRolesToUser(string userId, List<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                if(!await _roleManager.RoleExistsAsync(role))
                {
                    return false; //se le puede devolver un mensaje tipo "Error: El rol '*' con id '*' no existe."
                }
            }

            IdentityResult removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                return false; //mensaje tipo "Error al remover roles."
            }
            IdentityResult addResult = await _userManager.AddToRolesAsync(user, roles);
            if (!addResult.Succeeded)
            {
                await _userManager.AddToRolesAsync(user, currentRoles);

                return false; //mensaje tipo "Error al agregar roles."
            }

            return true;
        }

        public async Task<IList<string>> GetUsersRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;
            
            return await _userManager.GetRolesAsync(user);
        }

    }
}
