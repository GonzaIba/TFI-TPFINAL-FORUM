using Core.Domain.IdentityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Models
{
    public class PublicacionGuardadaModel
    {
        public int IDPublicacionGuardada { get; set; }
        public string IDUsuario { get; set; }
        public int IDPublicacion { get; set; }

        // Propiedades de navegación
        public Users Usuario { get; set; }
        public PublicacionModel Publicacion { get; set; }
    }
}
