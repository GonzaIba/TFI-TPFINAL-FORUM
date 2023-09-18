using Core.Domain.IdentityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Models
{
    public class UsuarioMedallaModel
    {
        public int IDUsuarioMedalla { get; set; }
        public string IDUsuario { get; set; }
        public int IDMedalla { get; set; }
        
        // Propiedades de navegación
        public Users Usuario { get; set; }
        public MedallaModel Medalla { get; set; }
    }
}
