using Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Response
{
    public class PublicationDetailResponse
    {
        public int CodigoPublicacion { get; set; }
        public UsersForumPreviewResponse Usuario { get; set; }
        public string Titulo { get; set; }
        public string Contenido { get; set; }
        public int Recompensa { get; set; }
        public int Visitas { get; set; }
        public int Votos { get; set; }
        public bool? VotadoPositivo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaCierre { get; set; }

        public List<AnswerResponse> Respuestas { get; set; } = new();
        public List<FilesResponse> Archivos { get; set; } = new();

        // Propiedades de navegación
        //public ICollection<EtiquetaPublicacionModel> EtiquetasPublicacion { get; set; }
        //public ICollection<RespuestaModel> Respuestas { get; set; }
        //public ICollection<ArchivoModel> Archivos { get; set; }
    }

    public class FilesResponse
    {
        public string NombreArchivo { get; set; }
        public string TipoArchivo { get; set; }
        public byte[] Archivo { get; set; }
    }

    public class AnswerResponse
    {
        public int CodigoRespuesta { get; set; }
        public UsersForumPreviewResponse Usuario { get; set; }
        public string TextoRespuesta { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool RespuestaCorrecta { get; set; }
        public int Votos { get; set; }
        public bool? VotadoPositivo { get; set; }
        public List<FilesResponse> Archivos { get; set; } = new();

        //public ICollection<RecompensaUsuarioModel> RecompensasUsuario { get; set; }
        //public ICollection<ArchivoModel> Archivos { get; set; }
        //public ICollection<RespuestaVotoModel> RespuestasVotos { get; set; }
    }
}
