using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.ML.TextoPrediccion
{
    public class TextoPrediccionInput
    {
        [ColumnName(@"Texto")]
        public string Texto { get; set; }

        [ColumnName(@"Etiquetas")]
        public string Etiquetas { get; set; }
    }
}
