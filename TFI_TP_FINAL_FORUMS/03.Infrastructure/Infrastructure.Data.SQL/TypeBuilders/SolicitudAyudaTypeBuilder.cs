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
    internal class SolicitudAyudaTypeBuilder : IEntityTypeConfiguration<SolicitudAyudaModel>
    {
        public void Configure(EntityTypeBuilder<SolicitudAyudaModel> builder)
        {
            builder.HasKey(e => e.IDSolicitudAyuda);
            builder.Property(e => e.IDSolicitudAyuda).ValueGeneratedOnAdd();
            builder.Property(e => e.Titulo).IsRequired().HasMaxLength(400);
            builder.Property(e => e.Descripcion).IsRequired(false);
            builder.Property(e => e.Urgencia).IsRequired(); // tinyint
            builder.Property(e => e.Lenguaje).HasMaxLength(20);
            builder.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
            builder.Property(e => e.RecompensaBase).HasPrecision(12, 2).IsRequired();
            builder.Property(e => e.IncrementoPorHora).HasPrecision(6, 4).IsRequired();
            builder.Property(e => e.FechaVencimiento).HasColumnType("timestamp(3) with time zone").IsRequired(); //.HasDefaultValueSql("DATEADD(HOUR,48,SYSUTCDATETIME())").IsRequired();
            builder.Property(e => e.IDUsuarioSolicitante).IsRequired().HasMaxLength(450);
            builder.Property(e => e.IDEstado).IsRequired();

            builder.HasOne(e => e.SolicitudAyudaEstado)
                   .WithMany(s => s.Solicitudes)
                   .HasForeignKey(e => e.IDEstado)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(e => e.SolicitudAyudaEtiquetas)
                   .WithOne(ep => ep.Solicitud)
                   .HasForeignKey(ep => ep.IDSolicitudAyuda)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Disponibilidades)
                   .WithOne(d => d.Solicitud)
                   .HasForeignKey(d => d.IDSolicitudAyuda)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Historial)
                   .WithOne(h => h.Solicitud)
                   .HasForeignKey(h => h.IDSolicitudAyuda)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(e => e.IDEstado).HasDatabaseName("IX_SA_IDEstado");
            builder.HasIndex(e => e.FechaVencimiento).HasDatabaseName("IX_SA_Vencimiento");
            builder.HasIndex(e => e.IDUsuarioSolicitante).HasDatabaseName("IX_SA_Solicitante");

            builder.ToTable("SolicitudAyuda");
        }
    }
}
