using Core.Domain.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Request
{
    public class ConfirmHelpRequest
    {
        public string UserId { get; set; }
        public TimeSlotItem TimeSlot { get; set; }
    }
}
