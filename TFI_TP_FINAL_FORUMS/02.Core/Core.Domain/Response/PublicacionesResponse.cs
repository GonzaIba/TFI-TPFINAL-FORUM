using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Response
{
    public class PublicacionesResponse
    {
        public int NumeroPublicacion { get; set; }
        public string CodigoUsuario { get; set; }
        public string Titulo { get; set; }
        public string Contenido { get; set; }
        public int Recompensa { get; set; }
        public int Visitas { get; set; }
        public int Respuestas { get; set; }
        public bool Respondida { get; set; }
        public bool Cerrada { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaCierre { get; set; }

        public List<string> Etiquetas { get; set; }
    }
}
