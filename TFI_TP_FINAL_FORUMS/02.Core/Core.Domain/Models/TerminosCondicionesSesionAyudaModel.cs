using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Models
{
    public class TerminosCondicionesSesionAyudaModel
    {
        public string UserId { get; set; }
        public int IdTyC { get; set; }
        public Guid IDSesion { get; set; }
        public bool Aceptado { get; set; }
        public int Version { get; set; }
        public DateTime FechaAceptado { get; set; }

        public SesionAyudaModel SesionAyuda { get; set; }
        public TerminosCondicionesModel TerminosCondiciones { get; set; }
    }
}
