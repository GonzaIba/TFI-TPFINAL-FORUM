using Core.Domain.IdentityModels;
using Core.Domain.Models.GenericEntityClass;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Models
{
    public class SolicitudAyudaModel : GenericEntity
    {
        public SolicitudAyudaModel()
        {
            SolicitudAyudaEtiquetas = new HashSet<SolicitudAyudaEtiquetasModel>();
            Disponibilidades = new HashSet<SolicitudAyudaDisponibilidadModel>();
            Historial = new HashSet<SolicitudAyudaHistorialModel>();
        }

        public int IDSolicitudAyuda { get; set; }
        public string Titulo { get; set; } = null!;
        public string? Descripcion { get; set; }
        public byte Urgencia { get; set; } = 1; // 0=Low,1=Normal,2=High
        public string? Lenguaje { get; set; }
        public string IDUsuarioSolicitante { get; set; } = null!;
        public int IDEstado { get; set; }
        public decimal RecompensaBase { get; set; }
        public decimal IncrementoPorHora { get; set; } // = 0.0200m;
        public DateTime FechaVencimiento { get; set; }
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        public virtual SolicitudAyudaEstadoModel? SolicitudAyudaEstado { get; set; }
        public virtual ICollection<SolicitudAyudaEtiquetasModel> SolicitudAyudaEtiquetas { get; set; } = new HashSet<SolicitudAyudaEtiquetasModel>();
        public virtual ICollection<SolicitudAyudaDisponibilidadModel> Disponibilidades { get; set; } = new HashSet<SolicitudAyudaDisponibilidadModel>();
        public virtual ICollection<SolicitudAyudaHistorialModel> Historial { get; set; } = new HashSet<SolicitudAyudaHistorialModel>();
        public virtual Users? UsuarioSolicitante { get; set; }
    }
}
