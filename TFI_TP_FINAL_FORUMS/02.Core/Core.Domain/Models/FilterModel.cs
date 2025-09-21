using Core.Domain.Models.GenericEntityClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Models
{
    public class FilterModel : GenericEntity
    {
        public int IDFilter { get; set; }
        public string Name { get; set; }
        public string Api { get; set; }
        public string Description { get; set; }
        public int IDFilterType { get; set; }
        public string? Options { get; set; } //Spliteados por coma.

        public ICollection<UserFiltersModel> UserFilters { get; set; } = new HashSet<UserFiltersModel>();
        public ICollection<GroupFiltersModel> GroupFilters { get; set; } = new HashSet<GroupFiltersModel>();
        public FilterTypeModel FilterType { get; set; }
    }
}