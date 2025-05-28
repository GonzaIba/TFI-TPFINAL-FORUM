using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Events
{
    public class AnswerVoteEvent
    {
        public int PublicationId { get; set; }
        public int AnswerId { get; set; }
        public int NewVoteCount { get; set; }
        public bool IsPositive { get; set; }
    }
}
