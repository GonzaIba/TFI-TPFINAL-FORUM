using Core.Domain.Models.GenericEntityClass;

namespace Core.Domain.Models
{
    public class EtiquetaModel : GenericEntity
    {
        public EtiquetaModel()
        {
            EtiquetasPublicaciones = new HashSet<EtiquetaPublicacionModel>();
        }
        public int IDEtiqueta { get; set; }
        public string NombreEtiqueta { get; set; }
        public string DescripcionEtiqueta { get; set; }

        // Propiedades de navegación
        public ICollection<EtiquetaPublicacionModel> EtiquetasPublicaciones { get; set; }
        public ICollection<SolicitudAyudaEtiquetasModel> SolicitudAyudaEtiquetas { get; set; }
    }
}
