using Core.Domain.Events;
namespace Core.Contracts.Publishers
{
    public interface IPublicationVotePublisher
    {
        Task PublishVotePublicationChangedAsync(PublicationVoteEvent message);
        Task PublishVoteAnswerChangedAsync(AnswerVoteEvent message);
        Task PublishAddAnswerChangedAsync(AddAnswerEvent message);
    }
}
