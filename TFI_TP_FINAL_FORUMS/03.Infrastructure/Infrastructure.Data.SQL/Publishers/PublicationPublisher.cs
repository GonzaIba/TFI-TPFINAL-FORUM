using Core.Contracts.Publishers;
using Core.Domain.Events;
using StackExchange.Redis;

namespace Infrastructure.Data.SQL.Publishers
{
    public class PublicationVotePublisher(IConnectionMultiplexer redis) : PublisherBase(redis), IPublicationVotePublisher
    {
        public Task PublishVotePublicationChangedAsync(PublicationVoteEvent message)
        {
            return PublishAsync("forum:votePublicationChanged", message);
        }

        public Task PublishVoteAnswerChangedAsync(AnswerVoteEvent message)
        {
            return PublishAsync("forum:voteAnswerChanged", message);
        }

        public Task PublishAddAnswerChangedAsync(AddAnswerEvent message)
        {
            return PublishAsync("forum:addAnswerChanged", message);
        }
    }
}
