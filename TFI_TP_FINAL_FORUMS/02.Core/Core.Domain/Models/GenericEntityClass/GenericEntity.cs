using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Models.GenericEntityClass
{
    public abstract class GenericEntity : IGenericEntity
    {
        [Column(TypeName = "boolean")]
        public bool Active { get; set; } = true;

        [Column(TypeName = "timestamp with time zone")]
        public DateTime CreateDate { get; set; }

        [Column(TypeName = "timestamp with time zone")]
        public DateTime? UpdateDate { get; set; }
    }
    
    public interface IGenericEntity
    {
        DateTime CreateDate { get; set; }
        DateTime? UpdateDate { get; set; }
        bool Active { get; set; }
    }
}
