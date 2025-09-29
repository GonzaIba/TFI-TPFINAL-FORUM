using Core.Domain.Models.GenericEntityClass;
using System;
using System.Collections.Generic;

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

        public virtual SolicitudAyudaModel Solicitud { get; set; } = null!;
        public virtual ICollection<SolicitudAyudaChatMensajeModel> Mensajes { get; set; }
    }
}

