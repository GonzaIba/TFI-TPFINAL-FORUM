using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Core.Domain.IdentityModels
{
    public class Users : IdentityUser
    {
        public bool Active { get; set; }
        public UsersRoles UserPrivileges { get; set; }
        public virtual ICollection<RefreshToken> UserRefreshTokens { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
