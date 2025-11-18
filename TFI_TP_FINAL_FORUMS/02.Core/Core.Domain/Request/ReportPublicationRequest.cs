namespace Core.Domain.Request
{
    public class ReportPublicationRequest
    {
        public int CodePublication { get; set; }
        public string UserId { get; set; }
        public string Reason { get; set; }
        public string Detail { get; set; }
    }
}
