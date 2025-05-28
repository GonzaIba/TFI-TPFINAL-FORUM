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

        public ICollection<UserFiltersModel> UserFilters { get; set; } = new HashSet<UserFiltersModel>();
        public ICollection<GroupFiltersModel> GroupFilters { get; set; } = new HashSet<GroupFiltersModel>();
    }
}