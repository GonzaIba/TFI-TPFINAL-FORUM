using Core.Domain.Models.GenericEntityClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Models
{
    public class SolicitudAyudaChatMensajeLecturaModel
    {
        public int IDMensaje { get; set; }
        public string IDUsuario { get; set; } = null!;

        public virtual SolicitudAyudaChatMensajeModel Mensaje { get; set; } = null!;
    }
}

