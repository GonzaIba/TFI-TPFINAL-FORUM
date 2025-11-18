namespace Core.Domain.Request
{
    public class ReportAnswerRequest
    {
        public int AnswerCode { get; set; }
        public string UserId { get; set; }
        public string Reason { get; set; }
        public string Detail { get; set; }
    }
}
