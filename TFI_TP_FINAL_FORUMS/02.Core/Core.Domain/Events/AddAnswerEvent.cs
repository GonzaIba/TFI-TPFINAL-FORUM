using Core.Domain.Response;

namespace Core.Domain.Events
{
    public class AddAnswerEvent : AnswerResponse
    {
        public string? ConnectionId { get; set; }
    }
}
