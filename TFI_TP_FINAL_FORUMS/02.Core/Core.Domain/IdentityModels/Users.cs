using System;
using System.Collections.Generic;
using System.Text;
using Core.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Core.Domain.IdentityModels
{
    public class Users
    {
        public Users()
        {
            UsersClaims = new HashSet<UsersClaims>();
            PublicacionesGuardadas = new HashSet<PublicacionGuardadaModel>();
            UsuarioMedallas = new HashSet<UsuarioMedallaModel>();
            UserRefreshTokens = new HashSet<RefreshToken>();
            RecompensasUsuarios = new HashSet<RecompensaUsuarioModel>();
        }

        // Propiedades básicas de IdentityUser
        public string Id { get; set; } // Puedes usar string o Guid según tu preferencia
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }


        //Propiedades Genéricas
        public string? LenguajePreferencia { get; set; }
        public DateTime FechaCreado { get; set; }
        public bool Active { get; set; }
        public string Nombre { get; set; }
        public string? Apellido { get; set; }


        //Para educacional
        //public string? ImageEducacional { get; set; }

        
        //Para los foros
        //public string? DescripcionCortaForum { get; set; }
        //public string? DescripcionLargaForum { get; set; }
        //public string? ImageForum { get; set; }
        //public DateTime UltimaVezConectadoForum { get; set; }

        
        //Propiedades para EF Core
        public UsersRoles UserPrivileges { get; set; }
        public UsersForumModel UsersForum { get; set; }
        public virtual ICollection<UsersClaims> UsersClaims { get; set; }
        public virtual ICollection<UsersLogin> UsersLogin { get; set; }
        public virtual ICollection<UsersToken> UsersTokens { get; set; }
        public virtual ICollection<RefreshToken> UserRefreshTokens { get; set; }
        public virtual ICollection<PublicacionGuardadaModel> PublicacionesGuardadas { get; set; }
        public virtual ICollection<UsuarioMedallaModel> UsuarioMedallas { get; set; }
        public virtual ICollection<RecompensaUsuarioModel> RecompensasUsuarios { get; set; }
        public virtual ICollection<RespuestaModel> Respuestas { get; set; }
        public virtual ICollection<PublicacionModel> Publicaciones { get; set; }

    }
}
