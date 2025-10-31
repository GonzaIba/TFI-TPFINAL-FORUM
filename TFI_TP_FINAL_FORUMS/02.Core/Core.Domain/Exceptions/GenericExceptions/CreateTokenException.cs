using Core.Domain.Exceptions.BaseException;

namespace Core.Domain.Exceptions.GenericExceptions
{
    public class CreateTokenException : ExceptionBase
    {
        private static string Code = "CTK-004";

        public CreateTokenException() : base(Code)
        {
        }
    }
}
