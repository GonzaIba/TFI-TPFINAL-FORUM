using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Response
{
    public class NotificationsResponse
    {
        public int CodeNotification { get; set; }
        public string CodeUser { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public bool Readed { get; set; }
    }
}
