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
    public class UsersClaimsTypeBuilder : IEntityTypeConfiguration<UsersClaims>
    {
        public void Configure(EntityTypeBuilder<UsersClaims> builder)
        {
            builder.HasKey(uc => uc.Id);
            
            builder.HasOne(uc => uc.User)
                .WithMany(u => u.UsersClaims)
                .HasForeignKey(uc => uc.UserId);

            builder.ToTable("UsersClaims");
        }
    }
}
