using Core.Domain.IdentityModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Contracts.Services
{
    public interface IRolesService : IGenericService<Roles>
    {
        public Task<bool> CreateRoleAsync(Roles roles);
        public Task<bool> DeleteRoleAsync(string id);
        public Task<Roles> UpdateRoleAsync(Roles roles);
        public Task<List<Roles>> GetRolesAsync();
        public Task<Roles> GetRoleByIdAsync(string id);
    }
}
