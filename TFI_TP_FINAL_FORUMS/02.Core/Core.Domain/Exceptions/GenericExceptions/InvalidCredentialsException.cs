
using Core.Domain.Exceptions.BaseException;
using System.Net;

namespace Core.Domain.Exceptions.GenericExceptions
{
    public class InvalidCredentialsException : ExceptionBase
    {
        private static string Code = "INC-002-UPI";
        private static string Description = "El usuario o la contraseña son incorrectos.";
        private static string Title = "Invalid Credentials";
        private static string Image = "";

        public InvalidCredentialsException() : base(Code, Description, Title, Image, nameof(InvalidCredentialsException), HttpStatusCode.InternalServerError)
        {
        }

        public InvalidCredentialsException(string description, string title, string nameError, HttpStatusCode statusCode) : base(Code, description, title, Image, nameError, statusCode)
        {
        }
    }
}
