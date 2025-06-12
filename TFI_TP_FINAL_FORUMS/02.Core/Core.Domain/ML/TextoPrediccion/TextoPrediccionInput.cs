using Microsoft.ML.Data;

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
