using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Response
{
    public class EtiquetasResponse
    {
        public EtiquetasResponse()
        {
            EtiquetasPublicaciones = new HashSet<EtiquetasPublicacionesResponse>();
        }
        public int CodigoEtiqueta { get; set; }
        public string NombreEtiqueta { get; set; }

        public IEnumerable<EtiquetasPublicacionesResponse> EtiquetasPublicaciones { get; set; }
    }
}
