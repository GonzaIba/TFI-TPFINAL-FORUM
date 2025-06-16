using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Request
{
    public class DeleteAnswerRequest
    {
        public int CodePublication { get; set; }
        public string? UserId { get; set; }
        public int AnswerCode { get; set; }
        public string? ConnectionId { get; set; }
        public DeleteAnswerRequest(int codePublication, string? userId, int answerCode, string? connectionId)
        {
            CodePublication = codePublication;
            UserId = userId;
            AnswerCode = answerCode;
            ConnectionId = connectionId;
        }
    }
}
