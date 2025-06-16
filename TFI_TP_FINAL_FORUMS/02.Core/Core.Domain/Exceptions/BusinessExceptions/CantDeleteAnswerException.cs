using Core.Domain.Exceptions.BaseException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Exceptions.BusinessExceptions
{
    public class CantDeleteAnswerException : ExceptionBase
    {
        private static string Code = "CDA-005-ANS";

        public CantDeleteAnswerException() : base(Code)
        {
        }
    }
}
