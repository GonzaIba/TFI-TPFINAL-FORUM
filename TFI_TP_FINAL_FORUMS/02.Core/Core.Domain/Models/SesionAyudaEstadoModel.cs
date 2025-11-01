using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Models
{
    public class SesionAyudaEstadoModel
    {
        public int IDEstado { get; set; }
        public string Estado { get; set; } = null!;

        public virtual ICollection<SesionAyudaModel> Sesiones { get; set; } = new HashSet<SesionAyudaModel>();
    }
}
