using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.GenericEntityClass;

namespace Core.Domain.Models
{
    public class ArchivoModel : GenericEntity
    {
        public int IDArchivo { get; set; }
        public int IDPublicacion { get; set; }
        public int IDRespuesta { get; set; }
        public string NombreArchivo { get; set; }
        public string TipoArchivo { get; set; }
        public byte[] Archivo { get; set; }

        // Propiedades de navegación
        public PublicacionModel Publicacion { get; set; }
        public RespuestaModel Respuesta { get; set; }
    }
}
