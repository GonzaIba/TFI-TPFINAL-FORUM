using Core.Domain.Response;

namespace Core.Contracts.Publishers
{
    public interface IPublisherPublication
    {
        Task PublishVotePublicationChangedAsync(int publicationId, int newVoteCount, string? connectionId);
        Task PublishVoteAnswerChangedAsync(int publicationId, int answerId, int newVoteCount, string? connectionId);
        Task PublishAddAnswerAsync(AnswerResponse answerResponse, string? connectionId, int publicationId);
        Task PublishDeleteAnswerAsync(int publicationId, int codeAnswer, string? connectionId);
    }
}
