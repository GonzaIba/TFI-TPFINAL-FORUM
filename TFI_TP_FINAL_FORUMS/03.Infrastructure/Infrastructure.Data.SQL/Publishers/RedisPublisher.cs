using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.SQL.Publishers
{
    public sealed class RedisPublisher : PublisherBase
    {
        public RedisPublisher(IConnectionMultiplexer redis) : base(redis) { }
    }
}
