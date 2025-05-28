using Core.Domain.Models.GenericEntityClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Models
{
    public class GroupModel : GenericEntity
    {
        public int IDGroup { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<GroupFiltersModel> GroupFilters { get; set; } = new HashSet<GroupFiltersModel>();
    }
}