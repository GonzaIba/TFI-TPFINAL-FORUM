using Core.Domain.Response;

namespace Core.Contracts.Services
{
    public interface IPublisherService
    {
        Task PublishVotePublicationChangedAsync(int publicationId, int newVoteCount, string? connectionId);
        Task PublishVoteAnswerChangedAsync(int publicationId, int answerId, int newVoteCount, string? connectionId);
        Task PublishAddAnswerChangedAsync(AnswerResponse answerResponse, string? connectionId);
    }
}
