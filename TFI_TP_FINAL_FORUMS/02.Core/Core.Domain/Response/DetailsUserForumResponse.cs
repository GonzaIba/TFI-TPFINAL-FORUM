using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Response
{
    public class DetailsUserForumResponse
    {
        public string Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? LenguajePreferencia { get; set; }
        public string? Email { get; set; }
        public DateTime FechaCreado { get; set; }
        public decimal Puntaje { get; set; }
        public int CantidadRespuestas { get; set; }
        public int CantidadPublicacionesCreadas { get; set; }


        public string? ShortDescriptionForum { get; set; }
        public string? LongDescriptionForum { get; set; }
        public string? ImageForum { get; set; }
        public DateTime LastTimeConnectedForum { get; set; }
        public List<Medalla> Medallas { get; set; }
    }

    public class Medalla
    {
        public string NombreMedalla { get; set; }
        public string Descripcion { get; set; }
        public string ImagenMedalla { get; set; }
        public DateTime FechaObtenido { get; set; }
    }
}
