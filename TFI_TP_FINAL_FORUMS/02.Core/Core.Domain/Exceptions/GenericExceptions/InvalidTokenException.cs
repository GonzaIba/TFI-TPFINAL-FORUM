using Core.Domain.Exceptions.BaseException;
using System.Net;

namespace Core.Domain.Exceptions.GenericExceptions
{
    public class InvalidTokenException : ExceptionBase
    {
        private static string Code = "TEX-001-RCA";
        private static string Description = "El token es inválido o expiró.";
        private static string Title = "Token Expired";
        private static string Image = "";

        public InvalidTokenException() : base(Code, Description, Title, Image, nameof(InvalidTokenException), HttpStatusCode.Unauthorized)
        {
        }

        public InvalidTokenException(string description, string title, string nameError, HttpStatusCode statusCode) : base(Code, description, title, Image, nameError, statusCode)
        {
        }
    }
}
