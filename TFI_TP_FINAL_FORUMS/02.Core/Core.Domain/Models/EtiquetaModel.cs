using Core.Domain.GenericEntityClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Models
{
    public class EtiquetaModel : GenericEntity
    {
        public EtiquetaModel()
        {
            EtiquetasPublicaciones = new HashSet<EtiquetaPublicacionModel>();
        }
        public int IDEtiqueta { get; set; }
        public string NombreEtiqueta { get; set; }

        // Propiedades de navegación
        public ICollection<EtiquetaPublicacionModel> EtiquetasPublicaciones { get; set; }
    }
}
