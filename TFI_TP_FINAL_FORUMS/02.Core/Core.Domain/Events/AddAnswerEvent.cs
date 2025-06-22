using Core.Domain.Response;

namespace Core.Domain.Events
{
    public class AddAnswerEvent : AnswerResponse
    {
        public int CodePublication { get; set; }
        public string? ConnectionId { get; set; }
    }
}
