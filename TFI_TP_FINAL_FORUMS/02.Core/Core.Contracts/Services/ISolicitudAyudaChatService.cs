using Core.Domain.Models;

namespace Core.Contracts.Services
{
    public interface ISolicitudAyudaChatService : IGenericService<SolicitudAyudaChatModel>
    {
        Task<SolicitudAyudaChatModel> GetOrCreateBySolicitudAsync(int idSolicitud);
        Task<SolicitudAyudaChatModel?> GetBySolicitudAsync(int idSolicitud);
    }
}
