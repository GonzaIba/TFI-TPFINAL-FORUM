using Core.Domain.DTOs;
using Core.Domain.IdentityModels;
using CrossCutting.Helpers.ResultClasses;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Contracts.Services
{
    public interface IUsersService : IGenericService<Users>
    {
        public Task<List<Users>> GetUsersAsync();
        public Task<Users> GetUserByNameAsync(string userName);
        public Task<bool> UpdateUserAsync(Users users);

        public Task<Dictionary<Users, int>> GetTopLastWeekAsync();
        public Task<IEnumerable<Users>> GetUsersForumAsync();
        public Task<Users> GetDetailUserAsync(string userEmail);
    }
}