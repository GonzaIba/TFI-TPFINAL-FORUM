using Core.Domain.Models.GenericEntityClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Models
{
    public class TextoPrediccionModel
    {
        public int IDTextoPrediccion { get; set; }
        public string Texto { get; set; }
        public string Etiquetas { get; set; }
    }
}
