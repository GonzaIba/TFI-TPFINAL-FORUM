using Core.Domain.Response;

namespace Core.Contracts.Services
{
    public interface IPublisherService
    {
        Task PublishVotePublicationChangedAsync(int publicationId, int newVoteCount);
        Task PublishVoteAnswerChangedAsync(int publicationId, int answerId, int newVoteCount);
        Task PublishAddAnswerChangedAsync(AnswerResponse answerResponse);
    }
}
