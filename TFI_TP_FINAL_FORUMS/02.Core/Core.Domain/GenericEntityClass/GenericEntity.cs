using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.GenericEntityClass
{
    public abstract class GenericEntity : IGenericEntity
    {       
        [Column(TypeName = "bit")]
        public bool Active { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime CreateDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? UpdateDate { get; set; }
    }
    
    public interface IGenericEntity
    {
        DateTime CreateDate { get; set; }
        Nullable<DateTime> UpdateDate { get; set; }
        bool Active { get; set; }
    }
}
