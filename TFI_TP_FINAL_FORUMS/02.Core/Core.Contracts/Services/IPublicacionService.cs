using Core.Domain.GenericEntityClass;
using Core.Domain.Models;
using Core.Domain.Request;
using Core.Domain.Response;

namespace Core.Contracts.Services
{
    public interface IPublicacionService : IGenericService<PublicacionModel>
    {
        Task<bool> CreatePublication(CreatePublicationRequest publication);
        Task<bool> EditPublication(string userId, EditPublicationRequest editPublication);
        Task<bool> EditAnswer(string userId, EditAnswerRequest editPublication);
        Task<bool> SavePublication(string userId, int codePublication);
        Task<RespuestaModel> AddAnswer(AddAnswerRequest request);
        Task<bool> DeleteSavedPublication(string userId, int codePublication);
        Task<PaginatedList<PublicacionModel>> GetPublications(int pageIndex, int pageCount);
        Task<PublicacionModel> GetDetailPublication(int codePublication);
        Task<PaginatedList<PublicacionModel>> GetCreatedPublicationByUser(int pageIndex, int pageCount, string userId);
        Task<PaginatedList<PublicacionModel>> GetSavedPublications(int pageIndex, int pageCount, string userId);
        Task<IEnumerable<string>> PredictLabel(string texto);
        Task<PaginatedList<PublicacionModel>> GetPublicationsByFilter(string rawQuery, int pageIndex, int pageCount);
        Task<AnswerPublicationVoteResponse> UserPublicationVote(PublicationVoteRequest request);
        Task<AnswerPublicationVoteResponse> UserAnswerVote(AnswerVoteRequest request);
        Task<IEnumerable<PublicacionModel>> GetRelatedPublications(int publicationCode);
        Task<IEnumerable<PublicacionModel>> GetTopPublications();
        Task<bool> DeleteAnswerByUser(DeleteAnswerRequest request);
        Task<bool> ReportPublication(ReportPublicationRequest request);
        Task<bool> ReportAnswer(ReportAnswerRequest request);
    }
}
