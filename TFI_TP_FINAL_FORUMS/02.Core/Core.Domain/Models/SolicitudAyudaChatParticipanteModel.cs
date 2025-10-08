using Core.Domain.IdentityModels;
using Core.Domain.Models.GenericEntityClass;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Models
{
    public class SolicitudAyudaChatParticipanteModel : GenericEntity
    {
        public int IDChat { get; set; }

        public string IDUsuario { get; set; } = null!;

        public byte Rol { get; set; }

        // Navs
        public virtual SolicitudAyudaChatModel Chat { get; set; } = null!;

        [NotMapped]
        public Users Usuario { get; set; } = null!;
    }
}
