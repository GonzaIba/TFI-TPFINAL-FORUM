using Core.Domain.GenericEntityClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        
        // Propiedades de navegación
        public ICollection<EtiquetaPublicacionModel> EtiquetasPublicacion { get; set; }
        public ICollection<RespuestaModel> Respuestas { get; set; }
        public ICollection<ArchivoModel> Archivos { get; set; }
        public ICollection<PublicacionGuardadaModel> PublicacionesGuardadas { get; set; }
    }
}
