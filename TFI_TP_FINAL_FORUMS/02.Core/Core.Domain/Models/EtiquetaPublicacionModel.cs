using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Models
{
    public class EtiquetaPublicacionModel
    {
        public int IDEtiquetaPublicacion { get; set; }
        public int IDPublicacion { get; set; }
        public int IDEtiqueta { get; set; }

        // Propiedades de navegación
        public PublicacionModel Publicacion { get; set; }
        public EtiquetaModel Etiqueta { get; set; }
    }
}
