using Core.Domain.Models;
using Core.Domain.Request;
using Core.Domain.Response;

namespace Core.Contracts.Services
{
    public interface IPublicacionService : IGenericService<PublicacionModel>
    {
        Task<bool> CreatePublication(string userId, PublicacionModel publication);
        Task<bool> SavePublication(string userId, int codePublication);
        Task<RespuestaModel> AddAnswer(AddAnswerRequest request);
        Task<bool> DeleteSavedPublication(string userId, int codePublication); 
        Task<IEnumerable<PublicacionModel>> GetPublications();
        Task<PublicacionModel> GetDetailPublication(int codePublication);
        Task<IEnumerable<PublicacionModel>> GetCreatedPublicationByUser(string userId);
        Task<IEnumerable<PublicacionModel>> GetSavedPublications(string userId);
        Task<IEnumerable<string>> PredictLabel(string texto);
        Task<IEnumerable<PublicacionModel>> GetPublicationByFilter(string texto);
        Task<AnswerPublicationVoteResponse> UserPublicationVote(PublicationVoteRequest request);
        Task<AnswerPublicationVoteResponse> UserAnswerVote(AnswerVoteRequest request);
    }
}