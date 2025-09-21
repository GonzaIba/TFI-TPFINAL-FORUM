using Core.Domain.GenericEntityClass;
using Core.Domain.Models;

namespace Core.Contracts.Services
{
    public interface ISolicitudAyudaService : IGenericService<SolicitudAyudaModel>
    {
        Task<CursorPage<SolicitudAyudaModel>> GetRequestsHelp(int limit, DateTime anchorUtc, (DateTime createdAt, int id)? after, string? userId, string? search);
    }
}
