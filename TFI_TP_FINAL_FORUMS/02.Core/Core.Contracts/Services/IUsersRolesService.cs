using Core.Domain.IdentityModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Contracts.Services
{
    public interface IUsersRolesService : IGenericService<UsersRoles>
    {
        public Task<bool> AssignRolesToUser(string userId, List<string> roles);
        public Task<IList<string>> GetUsersRoles(string userId);
    }
}
