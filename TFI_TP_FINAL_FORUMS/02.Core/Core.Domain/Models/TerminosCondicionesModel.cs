using Core.Domain.Models.GenericEntityClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Models
{
    public class TerminosCondicionesModel : GenericEntity
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Version { get; set; }

        public ICollection<TerminosCondicionesSesionAyudaModel> TerminosCondicionesSesionesAyuda { get; set; }
    }
}
