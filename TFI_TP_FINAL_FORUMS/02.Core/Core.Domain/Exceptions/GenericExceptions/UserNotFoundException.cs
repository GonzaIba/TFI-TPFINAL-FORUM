using Core.Domain.Exceptions.BaseException;

namespace Core.Domain.Exceptions.GenericExceptions
{
    public class UserNotFoundException : ExceptionBase
    {
        private static string Code = "UNF-004-UNE";
        private static string Description = "El usuario no se encuentra en la base de datos.";
        private static string Title = "User Not Found";
        private static string Image = "https://i.imgur.com/9iy2M2p.png";

        public UserNotFoundException() : base(Code, Description, Title, Image, nameof(UserNotFoundException), HttpStatusCode.NoContent)
        {
        }

        public UserNotFoundException(string description, string title, string nameError, HttpStatusCode statusCode) : base(Code, description, title, Image, nameError, statusCode)
        {
        }
    }
}