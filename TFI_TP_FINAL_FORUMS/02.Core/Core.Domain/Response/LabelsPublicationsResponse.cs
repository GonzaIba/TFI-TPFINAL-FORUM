using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Response
{
    public class LabelsPublicationsResponse
    {
        public PublicationResponse Publicacion { get; set; }
        public LabelResponse Etiqueta { get; set; }
    }
}
