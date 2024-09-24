using Core.Domain.GenericEntityClass;
using Core.Domain.IdentityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Models
{
    public class RecompensaUsuarioModel : GenericEntity
    {
        public int IDRecompensa { get; set; }
        public string IDUsuario { get; set; }
        public int IDRespuesta { get; set; }
        public DateTime FechaObtencion { get; set; }
        public int CantidadRecompensa { get; set; }

        // Propiedades de navegación
        public RespuestaModel Respuesta { get; set; }
        //public Users Usuario { get; set; }
    }
}
