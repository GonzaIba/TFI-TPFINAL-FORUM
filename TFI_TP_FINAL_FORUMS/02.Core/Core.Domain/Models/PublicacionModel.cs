using Core.Domain.IdentityModels;
using Core.Domain.Models.GenericEntityClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Models
{
    public class PublicacionModel : GenericEntity
    {
        public PublicacionModel()
        {
            EtiquetasPublicacion = new HashSet<EtiquetaPublicacionModel>();
            Respuestas = new HashSet<RespuestaModel>();
            Archivos = new HashSet<ArchivoModel>();
            PublicacionesGuardadas = new HashSet<PublicacionGuardadaModel>();
            PublicacionesVotos = new HashSet<PublicacionVotoModel>();
            Denuncias = new HashSet<DenunciaModel>();
        }
        public int IDPublicacion { get; set; }
        public string IDUsuario { get; set; }
        public string Titulo { get; set; }
        public string Contenido { get; set; }
        public int Recompensa { get; set; }
        public int Visitas { get; set; }
        public bool Respondida { get; set; }
        public bool Cerrada { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaCierre { get; set; }

        [NotMapped]
        public Users? Usuario { get; set; }

        public ICollection<EtiquetaPublicacionModel> EtiquetasPublicacion { get; set; }
        public ICollection<RespuestaModel> Respuestas { get; set; }
        public ICollection<ArchivoModel> Archivos { get; set; }
        public ICollection<PublicacionGuardadaModel> PublicacionesGuardadas { get; set; }
        public ICollection<PublicacionVotoModel> PublicacionesVotos { get; set; }
        public ICollection<DenunciaModel> Denuncias { get; set; }
    }
}
