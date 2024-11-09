using Core.Domain.GenericEntityClass;
using Core.Domain.IdentityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Models
{
    public class PublicacionVotoModel : GenericEntity
    {
        public int IDPublicacion { get; set; }
        public string IDUsuario { get; set; }
        public bool Positivo { get; set; }

        public PublicacionModel Publicacion { get; set; }
        public Users Usuario { get; set; }
    }
}
