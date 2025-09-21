using Core.Domain.Models.GenericEntityClass;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Models
{
    public class SolicitudAyudaDisponibilidadModel : GenericEntity
    {
        public SolicitudAyudaDisponibilidadModel()
        {
            Reservas = new HashSet<SolicitudAyudaReservaModel>();
        }

        public int IDDisponibilidad { get; set; }
        public int IDSolicitudAyuda { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Fin { get; set; }
        public byte Estado { get; set; } = 0; // 0=Disponible,1=Reservado,2=Confirmado,3=Usado,4=Cancelado,5=Expirado
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        public virtual SolicitudAyudaModel? Solicitud { get; set; }
        public virtual ICollection<SolicitudAyudaReservaModel> Reservas { get; set; } = new HashSet<SolicitudAyudaReservaModel>();
    }
}
