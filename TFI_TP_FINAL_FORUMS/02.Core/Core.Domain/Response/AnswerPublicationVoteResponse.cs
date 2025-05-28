using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Response
{
    public class AnswerPublicationVoteResponse : SuccessfulResponse
    {
        public AnswerPublicationVoteResponse(bool success, bool isVoteCreatedExpired) : base(success)
        {
            this.IsVoteCreatedExpired = isVoteCreatedExpired;
        }

        public bool IsVoteCreatedExpired { get; set; }
    }
}
