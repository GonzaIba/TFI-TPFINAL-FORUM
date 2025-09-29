using Core.Domain.IdentityModels;
using Core.Domain.Models.GenericEntityClass;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Models
{
    public class SolicitudAyudaChatMensajeModel : GenericEntity
    {
        public int IDMensaje { get; set; }
        public int IDChat { get; set; }
        public string IDUsuario { get; set; } = null!;
        public string Mensaje { get; set; } = null!;

        public virtual SolicitudAyudaChatModel Chat { get; set; } = null!;

        [NotMapped]
        public Users? Usuario { get; set; }

        public virtual ICollection<SolicitudAyudaChatMensajeLecturaModel> Lecturas { get; set; } = new HashSet<SolicitudAyudaChatMensajeLecturaModel>();

        [NotMapped]
        public bool LeidoPorUsuarioActual { get; set; }
    }
}
