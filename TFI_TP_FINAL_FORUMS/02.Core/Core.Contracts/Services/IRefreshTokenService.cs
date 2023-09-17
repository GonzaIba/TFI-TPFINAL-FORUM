using Core.Domain.IdentityModels;
using CrossCutting.Helpers.ResultClasses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Contracts.Services
{
    public interface IRefreshTokenService : IGenericService<RefreshToken>
    {
        public Task<IGenericResult> CreateAsync(string userId, string token);
        public Task<IGenericResult> ReplaceAsync(string userId, string oldToken, string newToken);

    }
}
