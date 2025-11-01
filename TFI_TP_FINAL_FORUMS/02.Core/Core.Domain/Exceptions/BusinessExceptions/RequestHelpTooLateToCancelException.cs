using Core.Domain.Exceptions.BaseException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Exceptions.BusinessExceptions
{
    public class RequestHelpTooLateToCancelException : ExceptionBase
    {
        private static string Code = "RTC-008-CXL";

        public RequestHelpTooLateToCancelException() : base(Code)
        {
        }
    }
}
