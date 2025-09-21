using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Models
{
    public class SolicitudAyudaEtiquetasModel
    {
        public int IDSolicitudAyudaEtiquetas { get; set; }
        public int IDSolicitudAyuda { get; set; }
        public int IDEtiqueta { get; set; }

        public virtual SolicitudAyudaModel? Solicitud { get; set; }
        public virtual EtiquetaModel? Etiqueta { get; set; }
    }
}
