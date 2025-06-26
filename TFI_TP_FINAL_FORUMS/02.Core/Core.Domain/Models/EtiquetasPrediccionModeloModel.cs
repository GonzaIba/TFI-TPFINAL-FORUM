using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Models
{
    public class EtiquetasPrediccionModeloModel
    {
        public int Id { get; set; }
        public byte[] ModelData { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
