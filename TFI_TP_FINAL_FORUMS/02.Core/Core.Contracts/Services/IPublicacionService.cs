using Core.Domain.Models;
using Core.Domain.Request;
using Core.Domain.Response;

namespace Core.Contracts.Services
{
    public interface IPublicacionService : IGenericService<PublicacionModel>
    {
        Task<bool> CreatePublication(string userId, PublicacionModel publication);
        Task<bool> EditPublication(string userId, EditPublicationRequest editPublication);
        Task<bool> EditAnswer(string userId, EditAnswerRequest editPublication);
        Task<bool> SavePublication(string userId, int codePublication);
        Task<RespuestaModel> AddAnswer(AddAnswerRequest request);
        Task<bool> DeleteSavedPublication(string userId, int codePublication); 
        Task<List<PublicacionModel>> GetPublications();
        Task<PublicacionModel> GetDetailPublication(int codePublication);
        Task<List<PublicacionModel>> GetCreatedPublicationByUser(string userId);
        Task<List<PublicacionModel>> GetSavedPublications(string userId);
        Task<IEnumerable<string>> PredictLabel(string texto);
        Task<IEnumerable<PublicacionModel>> GetPublicationByFilter(string texto);
        Task<AnswerPublicationVoteResponse> UserPublicationVote(PublicationVoteRequest request);
        Task<AnswerPublicationVoteResponse> UserAnswerVote(AnswerVoteRequest request);
        Task<IEnumerable<PublicacionModel>> GetRelatedPublications(int publicationCode);
        Task<IEnumerable<PublicacionModel>> GetTopPublications();
        Task<bool> DeleteAnswerByUser(DeleteAnswerRequest request);
    }
}