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
    public class TerminosCondicionesSesionAyudaTypeBuilder : IEntityTypeConfiguration<TerminosCondicionesSesionAyudaModel>
    {
        public void Configure(EntityTypeBuilder<TerminosCondicionesSesionAyudaModel> builder)
        {
            builder.HasKey(e => new { e.UserId, e.IdTyC, e.IDSesion });
            builder.Property(e => e.Aceptado)
                   .IsRequired();

            builder.Property(e => e.Version)
                   .IsRequired();

            builder.Property(e => e.FechaAceptado)
                   .IsRequired()
                   .HasColumnType("datetime2(3)");

            builder.HasOne(e => e.TerminosCondiciones)
                   .WithMany(t => t.TerminosCondicionesSesionesAyuda)
                   .HasForeignKey(e => e.IdTyC)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(e => e.SesionAyuda)
                   .WithMany(s => s.TerminosCondiciones)
                   .HasForeignKey(e => e.IDSesion)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.ToTable("TerminosCondicionesSesionAyuda");
        }
    }
}
