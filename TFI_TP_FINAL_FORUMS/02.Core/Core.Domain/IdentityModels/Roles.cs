using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.IdentityModels
{
    public class Roles
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
        public virtual ICollection<UsersRoles> UserRoles { get; set; }
        public virtual ICollection<RolesClaim> RoleClaims { get; set; }
    }
}
