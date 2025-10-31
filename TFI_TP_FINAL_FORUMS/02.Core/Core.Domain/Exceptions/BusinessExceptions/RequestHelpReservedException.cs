using Core.Domain.Exceptions.BaseException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Exceptions.BusinessExceptions
{
    public class RequestHelpReservedException : ExceptionBase
    {
        private static string Code = "RHR-007-RSV";

        public RequestHelpReservedException() : base(Code)
        {
        }
    }
}
