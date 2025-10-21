using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Request
{
    public class AcceptTyCRequest
    {
        public string UserId { get; set; }
        public int CodeRequestHelp { get; set; }
    }
}
