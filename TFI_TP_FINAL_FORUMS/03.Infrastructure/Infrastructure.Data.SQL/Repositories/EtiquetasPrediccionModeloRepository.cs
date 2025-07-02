using Core.Contracts.Repositories;
using Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.SQL.Repositories
{
    public class EtiquetasPrediccionModeloRepository : GenericRepository<EtiquetasPrediccionModeloModel>, IEtiquetasPrediccionModeloRepository
    {
        public EtiquetasPrediccionModeloRepository(ApplicationDbContext applicationDb) : base(applicationDb)
        {
            
        }

        public async Task<IEnumerable<EtiquetasPrediccionModeloModel>> GetAllAsync()
        {
            var result = await this.Entities
                        .OrderByDescending(m => m.CreateDate)
                        .ToListAsync();

            return result;
        }
    }
}
