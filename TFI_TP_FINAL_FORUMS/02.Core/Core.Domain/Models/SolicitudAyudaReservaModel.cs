using Core.Domain.IdentityModels;
using Core.Domain.Models.GenericEntityClass;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Models
{
    public class SolicitudAyudaReservaModel : GenericEntity
    {
        public int IDReserva { get; set; }
        public int IDDisponibilidad { get; set; }
        public string IDUsuarioAyudante { get; set; } = null!;
        public byte Estado { get; set; } = 0; // 0=Pendiente,1=Confirmada,2=EnCurso,3=Cancelada,4=Expirada,5=Finalizada

        public virtual SolicitudAyudaDisponibilidadModel? Disponibilidad { get; set; }
        public virtual Users? UsuarioAyudante { get; set; }
        public virtual SesionAyudaModel? Sesion { get; set; }
    }
}
