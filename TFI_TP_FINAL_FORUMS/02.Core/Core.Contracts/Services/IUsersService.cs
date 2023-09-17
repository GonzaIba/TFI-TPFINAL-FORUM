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
        public Task<IGenericResult<RegisterDto>> CreateUserAsync(Users users, string password);
        public Task<IdentityResult> ConfirmUserEmailAsync(string users, string token);
        public Task<IGenericResult<LoginTokenDto>> LoginUserAsync(string email, string password);
        public Task<IGenericResult<LoginTokenDto>> ExternalLoginUserAsync(ExternalLoginInfo info);
        public Task<List<Users>> GetUsersAsync();
        public Task<Users> GetUserByNameAsync(string userName);
        public Task ForgotPasswordGenerateToken(string userName);
        public Task ChangePasswordGenerateToken(ChangePasswordDto model);
        public Task<LoginTokenDto> GenerateRefreshToken(Users user, string token);
        public Task<bool> DeleteUserAsync(string id);
        public Task<bool> UpdateUserAsync(Users users);


    }
}