using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Response
{
    public class RequestHelpDetailResponse
    {
        public RequestHelpResponse RequestHelp { get; set; }
        public int? CodeChat { get; set; }
        public bool IsOwner { get; set; }
    }
}
