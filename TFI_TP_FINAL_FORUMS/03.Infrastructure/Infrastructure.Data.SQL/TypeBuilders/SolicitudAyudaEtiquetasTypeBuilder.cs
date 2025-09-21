using Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.SQL.TypeBuilders
{
    internal class SolicitudAyudaEtiquetasTypeBuilder : IEntityTypeConfiguration<SolicitudAyudaEtiquetasModel>
    {
        public void Configure(EntityTypeBuilder<SolicitudAyudaEtiquetasModel> builder)
        {
            builder.HasKey(e => new { e.IDSolicitudAyudaEtiquetas, e.IDSolicitudAyuda, e.IDEtiqueta });
            builder.Property(e => e.IDSolicitudAyudaEtiquetas).ValueGeneratedOnAdd();
            builder.Property(e => e.IDSolicitudAyuda).IsRequired();
            builder.Property(e => e.IDEtiqueta).IsRequired();
      
            builder.HasIndex(e => new { e.IDSolicitudAyuda, e.IDEtiqueta })
                   .IsUnique() // Unique lógico para evitar duplicados
                   .HasDatabaseName("UX_SolicitudAyudaEtiquetas");

            builder.HasOne(e => e.Solicitud)
                   .WithMany(s => s.SolicitudAyudaEtiquetas)
                   .HasForeignKey(e => e.IDSolicitudAyuda)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Etiqueta)
                   .WithMany(e => e.SolicitudAyudaEtiquetas)
                   .HasForeignKey(e => e.IDEtiqueta)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.ToTable("SolicitudAyudaEtiquetas");
        }
    }
}
