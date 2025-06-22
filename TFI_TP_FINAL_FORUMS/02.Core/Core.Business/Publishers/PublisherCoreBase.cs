using Core.Contracts.Publishers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Business.Publishers
{
    public class PublisherCoreBase : IPublisherCoreBase
    {
        private readonly IPublisherBase _IPublisherCoreBase;
        public PublisherCoreBase(IPublisherBase _IPublisherCoreBase)
        {
            this._IPublisherCoreBase = _IPublisherCoreBase ?? throw new ArgumentNullException(nameof(_IPublisherCoreBase));
        }
        public Task PublishAsync<T>(string channelName, T message)
        {
            if (string.IsNullOrWhiteSpace(channelName))
            {
                throw new ArgumentException("Channel name cannot be null or empty.", nameof(channelName));
            }
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message), "Message cannot be null.");
            }
            return _IPublisherCoreBase.PublishAsync(channelName, message);
        }
    }
}
