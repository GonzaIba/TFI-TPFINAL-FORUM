using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Request
{
    public class AddAnswerRequest
    {
        public int CodePublication { get; set; }
        public string UserId { get; set; }
        public string TextResponse { get; set; }
    }
}
