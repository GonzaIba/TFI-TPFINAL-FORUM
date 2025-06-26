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
            => await _entities
                        .OrderByDescending(m => m.CreatedOn)
                        .ToListAsync();
    }
}
