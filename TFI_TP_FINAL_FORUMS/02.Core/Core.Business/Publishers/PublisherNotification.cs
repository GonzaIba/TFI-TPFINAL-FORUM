using AutoMapper;
using Core.Contracts.Publishers;
using Core.Domain.Events;

namespace Core.Business.Publishers
{
    public class PublisherNotification : PublisherCoreBase, IPublisherNotification
    {
        private readonly IMapper _mapper;

        public PublisherNotification(IMapper mapper, IPublisherBase publisherBase) : base(publisherBase)
        {
            _mapper = mapper;
        }

        public Task AddNotification(int codeNotification, string codeUser, string message, DateTime date, bool readed)
        {
            var newNotificationEvent = new NewNotificationEvent
            {
                CodeNotification = codeNotification,
                CodeUser = codeUser,
                Message = message,
                Date = date,
                Readed = readed,
            };
            return PublishAsync("forum:notification:newNotification", newNotificationEvent);
        }

        public Task RemoveNotification(int codeNotification, string codeUser)
        {
            var removeNotificationEvent = new RemoveNotificationEvent
            {
                CodeNotification = codeNotification,
                CodeUser = codeUser,
            };
            return PublishAsync("forum:notification:removeNotification", removeNotificationEvent);
        }
    }
}
