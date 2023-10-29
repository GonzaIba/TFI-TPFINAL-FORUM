using System;
using System.Collections.Generic;
using System.Text;
using Core.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Core.Domain.IdentityModels
{
    public class Users : IdentityUser
    {
        public Users()
        {
            PublicacionesGuardadas = new HashSet<PublicacionGuardadaModel>();
            UsuarioMedallas = new HashSet<UsuarioMedallaModel>();
            UserRefreshTokens = new HashSet<RefreshToken>();
            RecompensasUsuarios = new HashSet<RecompensaUsuarioModel>();
        }
        public bool Active { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }

        //Para los foros
        public string ImageEducacional { get; set; }

        //Para los foros
        public string DescripcionCortaForum { get; set; }
        public string DescripcionLargaForum { get; set; }
        public string ImageForum { get; set; }
        public DateTime UltimaVezConectadoForum { get; set; }

        public UsersRoles UserPrivileges { get; set; }
        public virtual ICollection<RefreshToken> UserRefreshTokens { get; set; }
        public virtual ICollection<PublicacionGuardadaModel> PublicacionesGuardadas { get; set; }
        public virtual ICollection<UsuarioMedallaModel> UsuarioMedallas { get; set; }
        public virtual ICollection<RecompensaUsuarioModel> RecompensasUsuarios { get; set; }
    }
}
