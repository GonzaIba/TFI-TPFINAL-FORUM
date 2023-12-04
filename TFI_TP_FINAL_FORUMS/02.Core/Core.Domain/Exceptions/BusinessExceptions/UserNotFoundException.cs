using Core.Domain.Exceptions.BaseException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Exceptions.BusinessExceptions.Auth
{
    public class UserNotFoundException : ExceptionBase
    {
        private static string Code = "UNF-001";
        private static string Description = "El usuario no se encuentra en la base de datos.";
        private static string Title = "Usuario no encontrado";
        private static string Image = "https://i.imgur.com/9iy2M2p.png";

        public UserNotFoundException() : base(Code, Description, Title, Image)
        {
        }

        public UserNotFoundException(string description, string title) : base(Code, description, title, Image)
        {
        }
    }
}