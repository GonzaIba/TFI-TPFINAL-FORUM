using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Models
{
    public class SolicitudAyudaEstadoModel
    {
        public int IDEstado { get; set; }
        public string Estado { get; set; } = null!; // Activa, Reservada, EnCurso, Cerrada, Cancelada, Expirada

        public virtual ICollection<SolicitudAyudaModel> Solicitudes { get; set; } = new HashSet<SolicitudAyudaModel>();
    }
}
