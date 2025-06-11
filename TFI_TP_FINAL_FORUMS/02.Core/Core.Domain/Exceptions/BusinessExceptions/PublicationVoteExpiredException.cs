using Core.Domain.Exceptions.BaseException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Exceptions.BusinessExceptions
{
    public class PublicationVoteExpiredException : ExceptionBase
    {
        private static string Code = "PVE-002-PVE";

        public PublicationVoteExpiredException() : base(Code)
        {
        }
    }
}
