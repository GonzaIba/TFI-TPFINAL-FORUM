using Core.Domain.Models;

namespace Core.Contracts.Repositories
{
    public interface IEtiquetasPrediccionModeloRepository : IGenericRepository<EtiquetasPrediccionModeloModel>
    {
        public Task<IEnumerable<EtiquetasPrediccionModeloModel>> GetAllAsync();

    }
}
