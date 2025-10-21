using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Response
{
    public class GetSessionResponse
    {
        public string CodeSession { get; set; }
        public string Domain { get; set; }
        public string RoomName { get; set; }
        public DateTime InitAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsOwner { get; set; }
    }
}
