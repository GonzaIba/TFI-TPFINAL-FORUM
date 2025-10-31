using Core.Domain.Exceptions.BaseException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Exceptions.BusinessExceptions
{
    public class RequestHelpCantAccessException : ExceptionBase
    {
        private static string Code = "RCA-006-HLP";

        public RequestHelpCantAccessException() : base(Code)
        {
        }
    }
}
