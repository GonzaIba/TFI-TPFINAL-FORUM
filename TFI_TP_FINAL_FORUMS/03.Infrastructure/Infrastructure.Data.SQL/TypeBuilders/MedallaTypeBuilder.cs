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
    public class MedallaTypeBuilder : IEntityTypeConfiguration<MedallaModel>
    {
        public void Configure(EntityTypeBuilder<MedallaModel> builder)
        {
            builder.HasKey(m => m.IDMedalla);
            builder.Property(m => m.IDMedalla).IsRequired();
            builder.Property(m => m.NombreMedalla).IsRequired().HasMaxLength(100);
            builder.Property(m => m.Descripcion).IsRequired().HasMaxLength(1000);
            builder.Property(m => m.CantidadEntregada).IsRequired();
            builder.Property(m => m.ImagenMedalla).IsRequired().HasColumnType("text");

            builder.ToTable("Medallas");
        }
    }
}
