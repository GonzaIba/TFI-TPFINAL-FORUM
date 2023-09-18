using Core.Domain.GenericEntityClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Models
{
    public class RespuestaModel : GenericEntity
    {
        public RespuestaModel()
        {
            RecompensasUsuario = new HashSet<RecompensaUsuarioModel>();
            Archivos = new HashSet<ArchivoModel>();
        }
        public int IDRespuesta { get; set; }
        public int IDPublicacion { get; set; }
        public string IDUsuario { get; set; }
        public string TextoRespuesta { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool RespuestaCorrecta { get; set; }
        public int Votos { get; set; }

        // Propiedades de navegación
        public PublicacionModel Publicacion { get; set; }
        public ICollection<RecompensaUsuarioModel> RecompensasUsuario { get; set; }
        public ICollection<ArchivoModel> Archivos { get; set; }
    }
}
