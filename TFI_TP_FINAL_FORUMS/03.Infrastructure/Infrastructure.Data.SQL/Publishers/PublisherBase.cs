using Core.Contracts.Publishers;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Infrastructure.Data.SQL.Publishers
{
    public abstract class PublisherBase : IPublisherBase
    {
        private readonly IConnectionMultiplexer _redis;

        public PublisherBase(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task PublishAsync<T>(string channelName, T message)
        {
            var db = _redis.GetSubscriber();
            var serializedMessage = JsonConvert.SerializeObject(message);
            var channel = new RedisChannel(channelName, RedisChannel.PatternMode.Literal);
            await db.PublishAsync(channel, serializedMessage);
        }
    }
}
