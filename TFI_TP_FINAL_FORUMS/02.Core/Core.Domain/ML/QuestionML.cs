using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.ML
{
    public class QuestionML
    {
        [LoadColumn(0)]
        public string Text { get; set; }

        [LoadColumn(1)]
        public string Tags { get; set; }
    }
    public class QuestionPrediction
    {
        public string Tags { get; set; }
    }
}
