using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Request
{
    public class AnswerVoteRequest
    {
        public int publicationCode { get; set; }
        public int answerCode { get; set; }
        public bool isPositive { get; set; }
        public string? userId { get; set; }
    }
}
