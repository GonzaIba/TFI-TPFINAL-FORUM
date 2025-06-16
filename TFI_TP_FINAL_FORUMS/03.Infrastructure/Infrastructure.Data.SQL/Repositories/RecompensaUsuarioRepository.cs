using Core.Contracts.Repositories;
using Core.Domain.Models;
using Core.Domain.Views;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.SQL.Repositories
{
    public class RecompensaUsuarioRepository : GenericRepository<RecompensaUsuarioModel>, IRecompensaUsuarioRepository
    {
        public RecompensaUsuarioRepository(ApplicationDbContext applicationDbContext)
            : base(applicationDbContext)
        {

        }

        public async Task<List<TopThreeUsersLastWeekView>> GetTopThreeUsersLastWeek()
        {
            var topUsers = await _context.Set<TopThreeUsersLastWeekView>().AsNoTracking().ToListAsync();
            return topUsers;
        }
    }
}
