using AutoMapper;
using Core.Contracts.Publishers;
using Core.Domain.Events;
using Core.Domain.Response;

namespace Core.Business.Publishers
{
    public class PublisherPublication : PublisherCoreBase, IPublisherPublication
    {
        private readonly IMapper _mapper;

        public PublisherPublication(IMapper mapper, IPublisherBase publisherBase) : base(publisherBase)
        {
            _mapper = mapper;
        }

        public Task PublishVoteAnswerChangedAsync(int publicationId, int answerId, int newVoteCount, string? connectionId)
        {
            try
            {
                var answerVoteEvent = new AnswerVoteEvent
                {
                    CodePublication = publicationId,
                    AnswerId = answerId,
                    NewVoteCount = newVoteCount,
                    ConnectionId = connectionId,
                };
                return PublishAsync("forum:publication:voteAnswerChanged", answerVoteEvent);
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
                    CodePublication = publicationId,
                    NewVoteCount = newVoteCount,
                    ConnectionId = connectionId
                };
                return PublishAsync("forum:publication:votePublicationChanged", publicationVoteEvent);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task PublishAddAnswerAsync(AnswerResponse answerResponse, string connectionId, int publicationId)
        {
            try
            {
                var addAnswerEvent = _mapper.Map<AddAnswerEvent>(answerResponse);
                addAnswerEvent.ConnectionId = connectionId;
                addAnswerEvent.CodePublication = publicationId;
                return PublishAsync("forum:publication:addAnswer", addAnswerEvent);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task PublishDeleteAnswerAsync(int publicationId, int codeAnswer, string? connectionId)
        {
            try
            {
                var deleteAnswerEvent = new DeleteAnswerEvent
                {
                    CodePublication = publicationId,
                    CodeAnswer = codeAnswer,
                    ConnectionId = connectionId
                };
                return PublishAsync("forum:publication:deleteAnswer", deleteAnswerEvent);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task PublishEditAnswerAsync(int answerCode, string content, string? connectionId, int publicationId)
        {
            try
            {
                var editAnswerEvent = new EditAnswerEvent
                {
                    CodePublication = publicationId,
                    CodeAnswer = answerCode,
                    Content = content,
                    ConnectionId = connectionId
                };
                return PublishAsync("forum:publication:editAnswer", editAnswerEvent);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task PublishEditPublicationAsync(string content, string? connectionId, int publicationId)
        {
            try
            {
                var editAnswerEvent = new EditPublicationEvent
                {
                    CodePublication = publicationId,
                    Content = content,
                    ConnectionId = connectionId
                };
                return PublishAsync("forum:publication:editPublication", editAnswerEvent);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
