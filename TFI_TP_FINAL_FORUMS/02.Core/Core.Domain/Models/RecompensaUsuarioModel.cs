using Core.Domain.IdentityModels;
using Core.Domain.Models.GenericEntityClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Models
{
    public class RecompensaUsuarioModel : GenericEntity
    {
        public int IDRecompensaUsuario { get; set; }
        public string IDUsuario { get; set; }
        public int IDRecompensa { get; set; }
        public DateTime FechaObtencion { get; set; }

        public virtual RecompensaModel Recompensa { get; set; }
    }
}
