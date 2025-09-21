using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Response
{
    public class LabelResponse
    {
        public int CodeLabel { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CountThisWeek { get; set; }
        public int CountTotal {  get; set; }
    }
}
