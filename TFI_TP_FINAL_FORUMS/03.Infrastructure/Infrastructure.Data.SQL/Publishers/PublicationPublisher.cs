using Core.Contracts.Publishers;
using Core.Domain.Events;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.SQL.Publishers
{
    public class PublicationVotePublisher : IPublicationVotePublisher
    {
        private readonly IConnectionMultiplexer _redis;

        public PublicationVotePublisher(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task PublishVotePublicationChangedAsync(int publicationId, int newVoteCount)
        {
            var db = _redis.GetSubscriber();
            var message = new PublicationVoteEvent()
            {
                PublicationId = publicationId,
                NewVoteCount = newVoteCount
            };

            var serializedMessage = JsonConvert.SerializeObject(message);
            var channel = new RedisChannel("forum:votePublicationChanged", RedisChannel.PatternMode.Literal);
            await db.PublishAsync(channel, serializedMessage);
        }

        public async Task PublishVoteAnswerChangedAsync(int publicationId, int answerId, int newVoteCount)
        {
            var db = _redis.GetSubscriber();
            var message = new AnswerVoteEvent()
            {
                PublicationId = publicationId,
                AnswerId = answerId,
                NewVoteCount = newVoteCount,
            };

            var serializedMessage = JsonConvert.SerializeObject(message);
            var channel = new RedisChannel("forum:voteAnswerChanged", RedisChannel.PatternMode.Literal);
            await db.PublishAsync(channel, serializedMessage);
        }
    }
}
