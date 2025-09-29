using Core.Domain.Models;

namespace Core.Contracts.Services
{
    public interface ISolicitudAyudaChatMensajeService : IGenericService<SolicitudAyudaChatMensajeModel>
    {
        Task<IReadOnlyList<SolicitudAyudaChatMensajeModel>> GetMessagesAsync(int idChat, DateTime? afterUtc, int take, bool ascending, string currentUserId);
        Task<int> MarkAsReadAsync(int idChat, string userId, IEnumerable<int> messageIds);
        Task<int> MarkAllAsReadUpToAsync(int idChat, string userId, DateTime upToUtc);
        Task<int> CountUnreadAsync(int idChat, string userId);
    }
}
