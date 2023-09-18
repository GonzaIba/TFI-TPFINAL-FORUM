using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Exceptions
{
    [Serializable]
    public class ApiForumException : Exception
    {
        public ApiForumException() { }

        public ApiForumException(string message)
            : base(message) { }

        public ApiForumException(string message, Exception inner)
            : base(message, inner) { }
    }
}
