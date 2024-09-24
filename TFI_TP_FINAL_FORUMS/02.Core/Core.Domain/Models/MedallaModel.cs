using Core.Domain.GenericEntityClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Models
{
    public class MedallaModel : GenericEntity
    {
        public MedallaModel()
        {
            UsuariosMedallas = new HashSet<UsuarioMedallaModel>();
        }
        public int IDMedalla { get; set; }
        public string NombreMedalla { get; set; }
        public string Descripcion { get; set; }
        public int CantidadEntregada { get; set; }
        public string ImagenMedalla { get; set; }
        public bool Active { get; set; }

        // Propiedades de navegación
        public ICollection<UsuarioMedallaModel> UsuariosMedallas { get; set; }
    }
}
