using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Response
{
    public class UsuariosTopResponse
    {
        public string NombreCompleto { get; set; }
        public string Iniciales { get; set; }
        public string? DescripcionCorta { get; set; }
        public string? DescripcionLarga { get; set; }
        public string? Image { get; set; }
        public string? FechaDesde { get; set; }
        public int Puntaje { get; set; }
        public DateTime UltimaVezConectado { get; set; }
    }
}
