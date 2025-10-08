using System.Collections.Generic;
using Core.Domain.Models;

namespace Core.Contracts.Services
{
    public interface ISolicitudAyudaChatService : IGenericService<SolicitudAyudaChatModel>
    {
        Task<SolicitudAyudaChatModel?> GetByRequestCodeAndChatCodeAsync(int codeRequest, int codeChat);
        Task<SolicitudAyudaChatModel> GetRequestHelpChatAsync(int codeRequest, string requestingUserId, int codeChat);
        Task<int> CreateChatAsync(int codeRequest, string codeUser);
        Task<List<SolicitudAyudaChatModel>> ListMyHelpRequestChatsAsync(string codeUser, int codeRequest);
    }  
}
