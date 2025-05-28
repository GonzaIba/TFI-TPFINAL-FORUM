using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Models
{
    public class RecompensaModel
    {
        public int IDRecompensa { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Tipo { get; set; } // 'VotoPositivo', 'RespuestaCorrecta', 'Medalla', etc.
        public int Valor { get; set; } // opcional si querés que cada tipo tenga un valor asociado
        public DateTime CreateDate { get; set; } // fecha de creación de la recompensa

        public ICollection<RecompensaUsuarioModel> RecompensasUsuarios { get; set; } = new List<RecompensaUsuarioModel>();
    }
}
