using CrossCutting.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Response
{
    public class UsersForumResponse
    {
        public string Nombre { get; set; }
        public decimal Puntaje { get; set; }
        public DateTime FechaCreado { get; set; }
        public string Email { get; set; }
    }
}
