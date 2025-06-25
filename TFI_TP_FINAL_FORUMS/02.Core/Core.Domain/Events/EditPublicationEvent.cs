using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Events
{
    public class EditPublicationEvent
    {
        public int CodePublication { get; set; }
        public string Content { get; set; }
        public string? ConnectionId { get; set; }
    }
}
