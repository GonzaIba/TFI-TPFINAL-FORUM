using Core.Domain.IdentityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Models
{
    public class UserFiltersModel 
    {
        public int IDFilter { get; set; }
        public string UserId { get; set; }
        public string Value { get; set; }

        public virtual Users User { get; set; }
        public virtual FilterModel Filter { get; set; }
    }
}
