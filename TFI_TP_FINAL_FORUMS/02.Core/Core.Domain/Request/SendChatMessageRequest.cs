namespace Core.Domain.Request
{
    public class SendChatMessageRequest
    {
        public string UserId { get; set; } = null!;
        public int CodeChat { get; set; }
        public string Message { get; set; } = null!;
    }
}
