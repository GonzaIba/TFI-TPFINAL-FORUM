using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Request
{
    public class PublicationVoteRequest
    {
        public int CodePublication { get; set; }
        public bool IsPositive { get; set; }
        public string? UserId { get; set; }
        public string? ConnectionId { get; set; }
    }
}
