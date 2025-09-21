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
    internal class SolicitudAyudaDisponibilidadTypeBuilder : IEntityTypeConfiguration<SolicitudAyudaDisponibilidadModel>
    {
        public void Configure(EntityTypeBuilder<SolicitudAyudaDisponibilidadModel> builder)
        {
            builder.HasKey(e => e.IDDisponibilidad);
            builder.Property(e => e.IDDisponibilidad).ValueGeneratedOnAdd();
            builder.Property(e => e.IDSolicitudAyuda).IsRequired();
            builder.Property(e => e.Inicio).HasColumnType("datetime2(3)").IsRequired();
            builder.Property(e => e.Fin).HasColumnType("datetime2(3)").IsRequired();
            builder.Property(e => e.Estado).IsRequired();
            builder.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
            builder.Ignore(e => e.Active);

            builder.HasOne(e => e.Solicitud)
                   .WithMany(s => s.Disponibilidades)
                   .HasForeignKey(e => e.IDSolicitudAyuda)
                   .OnDelete(DeleteBehavior.Cascade);

            //builder.HasCheckConstraint("CK_SAD_Rango", "[InicioUtc] < [FinUtc]");

            builder.HasIndex(e => e.IDSolicitudAyuda).HasDatabaseName("IX_SAD_Solicitud");
            builder.HasIndex(e => new { e.Estado, e.Inicio }).HasDatabaseName("IX_SAD_EstadoTiempo");

            builder.ToTable("SolicitudAyudaDisponibilidad");
        }
    }
}
