using Core.Domain.IdentityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Models
{
    public class SolicitudAyudaHistorialModel
    {
        public int IDHistorial { get; set; }
        public int IDSolicitudAyuda { get; set; }
        public byte? EstadoAnterior { get; set; } // coincide con los TINYINT de tus tablas auxiliares
        public byte EstadoNuevo { get; set; }
        public string? Motivo { get; set; }
        public string? UserIdAccion { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual SolicitudAyudaModel? Solicitud { get; set; }
        public virtual Users? UsuarioAccion { get; set; }
    }
}
