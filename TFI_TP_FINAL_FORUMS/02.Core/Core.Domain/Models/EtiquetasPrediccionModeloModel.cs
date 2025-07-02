using Core.Domain.Models.GenericEntityClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Models
{
    public class EtiquetasPrediccionModeloModel : GenericEntity
    {
        public int Id { get; set; }
        public byte[] ModelData { get; set; }
    }
}
