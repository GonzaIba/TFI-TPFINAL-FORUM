using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.Domain.IdentityModels
{
    public class UsersRoles : IdentityUserRole<string>
    {
        public virtual ICollection<Users> Users { get; set; }
    }
}
