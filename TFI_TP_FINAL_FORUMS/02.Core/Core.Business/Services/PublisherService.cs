using Core.Contracts.Publishers;
using Core.Contracts.Services;
using Core.Domain.Events;
using Core.Domain.Response;
using AutoMapper;

namespace Core.Business.Services
{
    public class PublisherService : IPublisherService
    {
        private readonly IPublicationVotePublisher _publicationVotePublisher;
        private readonly IMapper _mapper;

        public PublisherService(IPublicationVotePublisher publicationVotePublisher, IMapper mapper)
        {
            _publicationVotePublisher = publicationVotePublisher;
            _mapper = mapper;
        }

        public Task PublishAddAnswerChangedAsync(AnswerResponse answerResponse, string connectionId)
        {
            try
            {
                var addAnswerEvent = _mapper.Map<AddAnswerEvent>(answerResponse);
                addAnswerEvent.ConnectionId = connectionId;
                return _publicationVotePublisher.PublishAddAnswerChangedAsync(addAnswerEvent);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task PublishVoteAnswerChangedAsync(int publicationId, int answerId, int newVoteCount, string? connectionId)
        {
            try
            {
                var answerVoteEvent = new AnswerVoteEvent
                {
                    PublicationId = publicationId,
                    AnswerId = answerId,
                    NewVoteCount = newVoteCount,
                    ConnectionId = connectionId,
                };
                return _publicationVotePublisher.PublishVoteAnswerChangedAsync(answerVoteEvent);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task PublishVotePublicationChangedAsync(int publicationId, int newVoteCount, string? connectionId)
        {
            try
            {
                var publicationVoteEvent = new PublicationVoteEvent
                {
                    PublicationId = publicationId,
                    NewVoteCount = newVoteCount,
                    ConnectionId = connectionId
                };
                return _publicationVotePublisher.PublishVotePublicationChangedAsync(publicationVoteEvent);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
