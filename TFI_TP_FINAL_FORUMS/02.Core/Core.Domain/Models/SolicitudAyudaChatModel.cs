using Core.Domain.IdentityModels;
using Core.Domain.Models.GenericEntityClass;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Models
{
    public class SolicitudAyudaChatModel : GenericEntity
    {
        public SolicitudAyudaChatModel()
        {
            Mensajes = new HashSet<SolicitudAyudaChatMensajeModel>();
        }

        public int IDChat { get; set; }
        public int IDSolicitudAyuda { get; set; }
        public string IDUsuarioAyudante { get; set; } = null!; //Confia en mi, no va a ser null cuando lo use...

        public virtual SolicitudAyudaModel Solicitud { get; set; } = null!;
        public virtual ICollection<SolicitudAyudaChatMensajeModel> Mensajes { get; set; }
        public virtual ICollection<SolicitudAyudaChatParticipanteModel> Participantes { get; set; } = new HashSet<SolicitudAyudaChatParticipanteModel>();

        [NotMapped]
        public Users? UsuarioAyudante { get; set; }
    }
}
