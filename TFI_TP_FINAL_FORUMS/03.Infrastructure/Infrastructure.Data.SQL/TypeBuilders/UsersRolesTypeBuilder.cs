using Core.Domain.IdentityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Data.SQL.TypeBuilders
{
    public class UsersRolesTypeBuilder : IEntityTypeConfiguration<UsersRoles>
    {
        public void Configure(EntityTypeBuilder<UsersRoles> builder)
        {
            builder.Ignore(x => x.Users);
            builder.ToTable("UsersRoles");
        }
    }
}
