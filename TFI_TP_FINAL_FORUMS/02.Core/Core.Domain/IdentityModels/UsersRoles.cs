using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.Domain.IdentityModels
{
    public class UsersRoles
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public Users User { get; set; }
        public Roles Role { get; set; }
    }
}
