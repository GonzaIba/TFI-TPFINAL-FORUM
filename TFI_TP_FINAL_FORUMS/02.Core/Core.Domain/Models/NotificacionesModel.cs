using Core.Domain.Models.GenericEntityClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Models
{
    public class NotificacionesModel : GenericEntity
    {
        public int IDNotificacion { get; set; }
        public string IDUsuario { get; set; }
        public string Mensaje { get; set; }
        public DateTime FechaNotificacion { get; set; }
        public bool Leida { get; set; }
    }
}
