using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Response
{
    public class SuccessfulResponse
    {
        public SuccessfulResponse(bool success)
        {
            Success = success;
        }

        public bool Success { get; set; }
    }
}
