using Core.Domain.GenericEntityClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Models
{
    public class GroupFiltersModel
    {
        public int IDGroup { get; set; }
        public int IDFilter { get; set; }

        public GroupModel Group { get; set; }
        public FilterModel Filter { get; set; }
    }
}
