using Core.Domain.Models.GenericEntityClass;
using System;

namespace Core.Domain.Models
{
    public class DenunciaModel : GenericEntity
    {
        public int IDDenuncia { get; set; }
        public int? IDPublicacion { get; set; }
        public int? IDRespuesta { get; set; }
        public string IDUsuarioReporto { get; set; }
        public string Motivo { get; set; }
        public string? Detalle { get; set; }
        public DateTime FechaDenuncia { get; set; }

        public PublicacionModel? Publicacion { get; set; }
        public RespuestaModel? Respuesta { get; set; }
    }
}
