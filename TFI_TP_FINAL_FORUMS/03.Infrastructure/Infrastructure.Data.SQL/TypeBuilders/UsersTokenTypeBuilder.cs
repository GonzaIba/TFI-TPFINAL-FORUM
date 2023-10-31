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
    public class UsersTokenTypeBuilder : IEntityTypeConfiguration<UsersToken>
    {
        public void Configure(EntityTypeBuilder<UsersToken> builder)
        {
            builder.HasKey(ut => new { ut.UserId, ut.LoginProvider, ut.Name });

            builder.HasOne(ut => ut.User)
                .WithMany(u => u.UsersTokens)
                .HasForeignKey(ut => ut.UserId);
            
            builder.ToTable("UsersToken");
        }
    }
}
