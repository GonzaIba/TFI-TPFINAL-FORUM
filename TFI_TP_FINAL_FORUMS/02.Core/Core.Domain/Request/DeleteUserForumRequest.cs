namespace Core.Domain.Request
{
    public class DeleteUserForumRequest
    {
        public string UserId { get; set; } = null!;
        public string? Reason { get; set; }
        public string? RequestedBy { get; set; }
    }
}
