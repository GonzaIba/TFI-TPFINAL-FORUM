using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Response
{
    public class LabelResponse
    {
        public LabelResponse()
        {
            EtiquetasPublicaciones = new HashSet<LabelsPublicationsResponse>();
        }
        public int CodigoEtiqueta { get; set; }
        public string NombreEtiqueta { get; set; }

        public IEnumerable<LabelsPublicationsResponse> EtiquetasPublicaciones { get; set; }
    }
}
