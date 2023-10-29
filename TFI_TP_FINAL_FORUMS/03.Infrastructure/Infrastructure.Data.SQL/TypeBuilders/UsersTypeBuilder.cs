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
            //builder.HasKey(x => x.Id);

            //builder.Property(x => x.Active)
            //       .IsRequired();

            builder.Ignore(x => x.UserPrivileges);

            builder.Property(x => x.Nombre)
                   .HasColumnType("varchar(50)");

            builder.Property(x => x.Apellido)
                   .HasColumnType("varchar(50)");

            //Educational
            builder.Property(x => x.ImageEducacional)
                   .HasColumnType("varchar(max)");

            //Forum
            builder.Property(x => x.DescripcionCortaForum)
                   .HasColumnType("varchar(50)");

            builder.Property(x => x.DescripcionLargaForum)
                   .HasColumnType("varchar(500)");

            builder.Property(x => x.ImageForum)
                   .HasColumnType("varchar(max)");

            builder.ToTable("Users");
        }
    }
}
