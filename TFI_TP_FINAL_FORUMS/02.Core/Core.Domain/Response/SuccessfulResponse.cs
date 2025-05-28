using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Response
{
    public partial class SuccessfulResponse
    {
        public SuccessfulResponse(bool success)
        {
            this.Success = success;
        }

        public bool Success { get; set; }
    }
}
