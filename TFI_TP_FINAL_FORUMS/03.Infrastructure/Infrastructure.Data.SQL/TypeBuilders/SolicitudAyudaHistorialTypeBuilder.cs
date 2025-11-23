using Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Data.SQL.TypeBuilders
{
    internal class SolicitudAyudaHistorialTypeBuilder : IEntityTypeConfiguration<SolicitudAyudaHistorialModel>
    {
        public void Configure(EntityTypeBuilder<SolicitudAyudaHistorialModel> builder)
        {
            builder.HasKey(e => e.IDHistorial);
            builder.Property(e => e.IDHistorial).ValueGeneratedOnAdd();

            builder.Property(e => e.IDSolicitudAyuda).IsRequired();
            builder.Property(e => e.EstadoAnterior).IsRequired(false);
            builder.Property(e => e.EstadoNuevo).IsRequired();
            builder.Property(e => e.Motivo).HasMaxLength(300);
            builder.Property(e => e.UserIdAccion).HasMaxLength(450);
            builder.Property(e => e.CreateDate).HasColumnType("timestamp(3) with time zone");

            builder.HasOne(e => e.Solicitud)
                   .WithMany(s => s.Historial)
                   .HasForeignKey(e => e.IDSolicitudAyuda)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(e => e.UsuarioAccion)
                   .WithMany(y=> y.SolicitudAyudaHistorial)
                   .HasForeignKey(e => e.UserIdAccion)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(e => new { e.IDSolicitudAyuda, e.CreateDate })
                   .HasDatabaseName("IX_SAH_Solicitud");

            builder.ToTable("SolicitudAyudaHistorial");
        }
    }
}