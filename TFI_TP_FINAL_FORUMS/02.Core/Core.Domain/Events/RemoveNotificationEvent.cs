using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Events
{
    public class RemoveNotificationEvent
    {
        public int CodeNotification { get; set; }
        public string CodeUser { get; set; }
    }
}
