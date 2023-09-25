using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.ML.TextoPrediccion
{
    public class TextoPrediccionOutput
    {
        [ColumnName(@"Pregunta")]
        public float[] Pregunta { get; set; }

        [ColumnName(@"Etiquetas")]
        public uint Etiquetas { get; set; }

        [ColumnName(@"Features")]
        public float[] Features { get; set; }

        [ColumnName(@"PredictedLabel")]
        public string PredictedLabel { get; set; }

        [ColumnName(@"Score")]
        public float[] Score { get; set; }
    }
}
