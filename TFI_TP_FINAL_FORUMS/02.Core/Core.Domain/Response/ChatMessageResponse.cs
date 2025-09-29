namespace Core.Domain.Response
{
    public class ChatMessageResponse
    {
        public int CodeMessage { get; set; }
        public int CodeChat { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Readed { get; set; }
        public bool SentByMe { get; set; }
    }
}
