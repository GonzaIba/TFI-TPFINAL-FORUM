using Core.Domain.GenericEntityClass;
using Core.Domain.Models;
using Core.Domain.Request;

namespace Core.Contracts.Services
{
    public interface ISolicitudAyudaService : IGenericService<SolicitudAyudaModel>
    {
        Task<CursorPage<SolicitudAyudaModel>> GetRequestsHelp(int limit, DateTime anchorUtc, (DateTime createdAt, int id)? after, string? userId, string? search);
        Task<List<SolicitudAyudaModel>> GetMyRequestsHelp(string? userId);
        Task<bool> CreateHelpRequest(CreateHelpRequest request);
        Task<bool> UpdateDisponibility(int id, UpdateDisponibilityRequest request);
        Task<(SolicitudAyudaModel, int?)> GetDetailRequestsHelp(int codeRequest, string codeUser);
        Task<bool> ConfirmRequestHelp(int codeRequest, ConfirmHelpRequest request);
        Task<bool> CancelRequestHelp(int codeRequest, string userId);
        Task<List<SolicitudAyudaModel>> GetRequestsHelpConfirmed(string? userId);
    }
}

