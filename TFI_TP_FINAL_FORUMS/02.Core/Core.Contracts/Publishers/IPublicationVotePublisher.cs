using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Contracts.Publishers
{
    public interface IPublicationVotePublisher
    {
        Task PublishVotePublicationChangedAsync(int publicationId, int newVoteCount);
        Task PublishVoteAnswerChangedAsync(int publicationId, int answerId, int newVoteCount);
    }
}
