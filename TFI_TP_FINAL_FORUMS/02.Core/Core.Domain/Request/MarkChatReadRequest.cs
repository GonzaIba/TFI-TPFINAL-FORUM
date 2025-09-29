namespace Core.Domain.Request
{
    public class MarkChatReadRequest
    {
        public string UserId { get; set; } = null!;
        public DateTime? UpToUtc { get; set; }
        public List<int>? MessageIds { get; set; }
    }
}

