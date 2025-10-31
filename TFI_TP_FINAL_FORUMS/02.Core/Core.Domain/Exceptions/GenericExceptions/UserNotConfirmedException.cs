using Core.Domain.Exceptions.BaseException;

namespace Core.Domain.Exceptions.GenericExceptions
{
    public class UserNotConfirmedException : ExceptionBase
    {
        private static string Code = "UNC-003-ENC";
        private static string Description = "El usuario no tiene el mail confirmado.";
        private static string Title = "User Not Confirmed";
        private static string Image = "";

        public UserNotConfirmedException() : base(Code, Description, Title, Image, nameof(UserNotConfirmedException), HttpStatusCode.InternalServerError)
        {
        }

        public UserNotConfirmedException(string description, string title, string nameError, HttpStatusCode statusCode) : base(Code, description, title, Image, nameError, statusCode)
        {
        }
    }
}
