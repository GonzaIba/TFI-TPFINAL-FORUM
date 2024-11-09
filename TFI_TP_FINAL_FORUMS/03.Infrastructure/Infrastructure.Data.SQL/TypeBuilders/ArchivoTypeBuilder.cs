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
    public class ArchivoTypeBuilder : IEntityTypeConfiguration<ArchivoModel>
    {
        public void Configure(EntityTypeBuilder<ArchivoModel> builder)
        {
            builder.HasKey(a => a.IDArchivo);
            builder.Property(a => a.IDArchivo).IsRequired();
            builder.Property(a => a.IDPublicacion).IsRequired();
            builder.Property(a => a.IDRespuesta).IsRequired();
            builder.Property(a => a.NombreArchivo).IsRequired().HasMaxLength(100);
            builder.Property(a => a.TipoArchivo).IsRequired().HasMaxLength(50);
            builder.Property(a => a.Archivo).IsRequired();

            builder.HasOne(a => a.Publicacion)
                .WithMany(p => p.Archivos)
                .HasForeignKey(a => a.IDPublicacion)
                .OnDelete(DeleteBehavior.NoAction); // Evitar cascada

            builder.HasOne(a => a.Respuesta)
                .WithMany(r => r.Archivos)
                .HasForeignKey(a => a.IDRespuesta)
                .OnDelete(DeleteBehavior.NoAction); // Evitar cascada

            builder.ToTable("Archivos");
        }
    }
}
