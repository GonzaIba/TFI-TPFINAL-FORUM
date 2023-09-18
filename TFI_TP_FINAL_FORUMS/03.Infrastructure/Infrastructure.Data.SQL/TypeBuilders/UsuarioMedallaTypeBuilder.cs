using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Models;

namespace Infrastructure.Data.SQL.TypeBuilders
{
    public class UsuarioMedallaTypeBuilder : IEntityTypeConfiguration<UsuarioMedallaModel>
    {
        public void Configure(EntityTypeBuilder<UsuarioMedallaModel> builder)
        {
            builder.HasKey(um => um.IDUsuarioMedalla);
            builder.Property(um => um.IDUsuarioMedalla).IsRequired();
            builder.Property(um => um.IDUsuario).IsRequired().HasMaxLength(450);
            builder.Property(um => um.IDMedalla).IsRequired();

            builder.HasOne(um => um.Usuario)
                .WithMany(u => u.UsuarioMedallas)
                .HasForeignKey(um => um.IDUsuario);

            builder.HasOne(um => um.Medalla)
                .WithMany(m => m.UsuariosMedallas)
                .HasForeignKey(um => um.IDMedalla);
            builder.ToTable("UsuariosMedallas");
        }
    }
}
