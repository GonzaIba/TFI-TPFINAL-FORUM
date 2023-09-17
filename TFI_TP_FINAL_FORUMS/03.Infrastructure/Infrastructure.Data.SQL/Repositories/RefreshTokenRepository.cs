using Core.Contracts.Repositories;
using Core.Domain.IdentityModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.SQL.Repositories
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(ApplicationDbContext applicationDbContext)
            : base(applicationDbContext)
        {

        }
        public async Task<RefreshToken> GetMatch(string userId, string token)
           => await Entities.SingleOrDefaultAsync(e => e.UserId == userId && e.Token == token);
    }
}
