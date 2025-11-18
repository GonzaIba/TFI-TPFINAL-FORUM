using Core.Domain.IdentityModels;
using Core.Domain.Models.GenericEntityClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Models
{
    public class RespuestaModel : GenericEntity
    {
        public RespuestaModel()
        {
            Archivos = new HashSet<ArchivoModel>();
            RespuestasVotos = new HashSet<RespuestaVotoModel>();
            Denuncias = new HashSet<DenunciaModel>();
        }

        public int IDRespuesta { get; set; }
        public int IDPublicacion { get; set; }
        public string IDUsuario { get; set; }
        public string TextoRespuesta { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool RespuestaCorrecta { get; set; }

        [NotMapped]
        public Users? Usuario { get; set; }

        // Propiedades de navegación
        public PublicacionModel Publicacion { get; set; }
        public ICollection<ArchivoModel> Archivos { get; set; }
        public ICollection<RespuestaVotoModel> RespuestasVotos { get; set; }
        public ICollection<DenunciaModel> Denuncias { get; set; }
    }
}
