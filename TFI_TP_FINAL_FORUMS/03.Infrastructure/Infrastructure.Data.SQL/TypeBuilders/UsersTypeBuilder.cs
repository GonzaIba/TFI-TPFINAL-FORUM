using Core.Domain.IdentityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Data.SQL.TypeBuilders
{
    public class UsersTypeBuilder : IEntityTypeConfiguration<Users>
    {
        public void Configure(EntityTypeBuilder<Users> builder)
        {

            builder.Ignore(x => x.UserPrivileges);

            builder.Property(x => x.FechaCreado)
                   .HasColumnType("datetime2(7)")
                   .HasDefaultValueSql("GETDATE()");

            builder.Property(x => x.Nombre)
                   .HasColumnType("varchar(50)");

            builder.Property(x => x.Apellido)
                   .HasColumnType("varchar(50)");

            //builder.HasOne(x => x.Country)
            //       .WithMany(z => z.Users)
            //       .HasForeignKey(y => y.IDCountry);

            builder.HasOne(x => x.UsersForum)
                   .WithOne(z => z.User);

            builder.ToTable("Users");
        }
    }
}
