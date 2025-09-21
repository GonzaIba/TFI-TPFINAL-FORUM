using Core.Domain.Enum;
using Core.Domain.GenericEntityClass;
using Core.Domain.Models;

namespace Core.Contracts.Services
{
    public interface IEtiquetaService : IGenericService<EtiquetaModel>
    {
        Task<PaginatedList<EtiquetaModel>> GetLabelsByName(string rawqQuery, int pageIndex, int pageCount);
        Task<PaginatedList<EtiquetaModel>> GetLabelsByFilter(LabelFiltersEnum filter, int pageIndex, int pageCount);     
        Task<bool> CreateLabel(string nombreEtiqueta);
    }
}
