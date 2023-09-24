using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Response
{
    public class EtiquetasPublicacionesResponse
    {
        public PublicacionesResponse Publicacion { get; set; }
        public EtiquetasResponse Etiqueta { get; set; }
    }
}
