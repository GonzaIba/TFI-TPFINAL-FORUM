using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Models
{
    public class FilterTypeModel
    {
        public FilterTypeModel()
        {
            Filters = new HashSet<FilterModel>();
        }

        public int IDFilterType { get; set; }
        public string Type { get; set; } // INSERT INTO[FilterType] VALUES('STRING'), ('BOOL'), ('SELECT'), ('DATE')

        public ICollection<FilterModel> Filters { get; set; }
    }
}
