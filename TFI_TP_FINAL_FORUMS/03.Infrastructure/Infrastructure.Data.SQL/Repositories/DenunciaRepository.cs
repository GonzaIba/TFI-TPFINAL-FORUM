using Core.Contracts.Repositories;
using Core.Domain.Models;

namespace Infrastructure.Data.SQL.Repositories
{
    public class DenunciaRepository : GenericRepository<DenunciaModel>, IDenunciaRepository
    {
        public DenunciaRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
