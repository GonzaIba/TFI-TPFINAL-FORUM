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
    public class UsersLoginTypeBuilder : IEntityTypeConfiguration<UsersLogin>
    {
        public void Configure(EntityTypeBuilder<UsersLogin> builder)
        {
            builder.HasKey(ul => new { ul.LoginProvider, ul.ProviderKey });

            builder.HasOne(ul => ul.User)
                .WithMany(u => u.UsersLogin)
                .HasForeignKey(ul => ul.UserId);

            builder.ToTable("UsersLogin");
        }
    }
}
