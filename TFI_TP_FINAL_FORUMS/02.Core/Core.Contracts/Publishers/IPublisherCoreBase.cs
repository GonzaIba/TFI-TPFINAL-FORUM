using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Contracts.Publishers
{
    public interface IPublisherCoreBase
    {
        Task PublishAsync<T>(string channelName, T message);
    }
}
