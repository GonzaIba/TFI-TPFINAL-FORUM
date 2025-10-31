using Core.Domain.Exceptions.BaseException;
using System.Net;

namespace Core.Domain.Exceptions.GenericExceptions
{
    public class UserRegisterCreateException : ExceptionBase
    {
        private static string Code = "URF-005-CEX";
        private static string Description = "Ocurrió un error al intentar crear la cuenta";
        private static string Title = "User Registration Failed";
        private static string Image = "";

        public UserRegisterCreateException() : base(Code, Description, Title, Image, nameof(UserRegisterCreateException), HttpStatusCode.InternalServerError)
        {
        }
        
        public UserRegisterCreateException(string description, string title, string nameError, HttpStatusCode statusCode) : base(Code, description, title, Image, nameError, statusCode)
        {
        }
    }
}
