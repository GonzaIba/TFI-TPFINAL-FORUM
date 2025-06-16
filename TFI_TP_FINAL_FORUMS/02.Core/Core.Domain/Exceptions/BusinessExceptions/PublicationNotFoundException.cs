using Core.Domain.Exceptions.BaseException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Exceptions.BusinessExceptions
{
    public class PublicationNotFoundException : ExceptionBase
    {
        private static string Code = "PNF-004-NFD";

        public PublicationNotFoundException() : base(Code)
        {
        }
    }
}
