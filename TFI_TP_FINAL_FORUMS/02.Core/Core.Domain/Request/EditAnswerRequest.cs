using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Request
{
    public class EditAnswerRequest
    {
        public int CodePublication { get; set; }
        public int AnswerCode { get; set; }
        public string Contenido { get; set; }
        public string? ConnectionId { get; set; }
    }
}
