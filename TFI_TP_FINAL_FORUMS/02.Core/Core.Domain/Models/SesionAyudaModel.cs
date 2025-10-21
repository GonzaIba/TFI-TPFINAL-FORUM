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
    public class SesionAyudaModel : GenericEntity
    {
        public Guid IDSesion { get; set; }
        public int IDReserva { get; set; }
        public string Estado { get; set; }
        public string Dominio { get; set; } //= "meet.jit.si";
        public string NombreSala { get; set; } = null!;
        public DateTime Inicio { get; set; }
        public DateTime Fin { get; set; }

        public virtual SolicitudAyudaReservaModel? Reserva { get; set; }
        //public virtual ICollection<SesionAyudaParticipanteModel>? Participantes { get; set; }
        public virtual ICollection<TerminosCondicionesSesionAyudaModel>? TerminosCondiciones { get; set; }
    }
}
