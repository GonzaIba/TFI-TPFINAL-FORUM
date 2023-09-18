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
        }
        public bool Active { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public UsersRoles UserPrivileges { get; set; }
        public virtual ICollection<RefreshToken> UserRefreshTokens { get; set; }
        public virtual ICollection<PublicacionGuardadaModel> PublicacionesGuardadas { get; set; }
        public virtual ICollection<UsuarioMedallaModel> UsuarioMedallas { get; set; }
    }
}
