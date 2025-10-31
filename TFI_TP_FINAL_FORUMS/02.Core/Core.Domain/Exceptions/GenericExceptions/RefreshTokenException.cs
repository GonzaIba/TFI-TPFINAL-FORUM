using Core.Domain.Exceptions.BaseException;
using System.Net;

namespace Core.Domain.Exceptions.GenericExceptions
{
    public class RefreshTokenException : ExceptionBase
    {
        private static string Code = "RFT-006-RLP";
        private static string Description = "El token de refresco es inválido o expiró.";
        private static string Title = "RefreshToken Error";
        private static string Image = "";

        public RefreshTokenException() : base(Code, Description, Title, Image, nameof(RefreshTokenException), HttpStatusCode.Forbidden)
        {
        }

        public RefreshTokenException(string description, string title, string nameError, HttpStatusCode statusCode) : base(Code, description, title, Image, nameError, statusCode)
        {
        }
    }
}
