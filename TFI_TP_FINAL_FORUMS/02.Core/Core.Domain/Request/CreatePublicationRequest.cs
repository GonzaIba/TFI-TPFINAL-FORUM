using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Request
{
    public class CreatePublicationRequest
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public List<string> Labels { get; set; }
        public string? UserId { get; set; }
    }
}
