using Core.Domain.IdentityModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.SQL.TypeBuilders
{
    public class RolesClaimTypeBuilder : IEntityTypeConfiguration<RolesClaim>
    {
        public void Configure(EntityTypeBuilder<RolesClaim> builder)
        {
            builder.HasKey(rc => rc.Id);

            builder.HasOne(rc => rc.Role)
                .WithMany(r => r.RoleClaims)
                .HasForeignKey(rc => rc.RoleId);

            builder.ToTable("RolesClaim");
        }
    }
}
