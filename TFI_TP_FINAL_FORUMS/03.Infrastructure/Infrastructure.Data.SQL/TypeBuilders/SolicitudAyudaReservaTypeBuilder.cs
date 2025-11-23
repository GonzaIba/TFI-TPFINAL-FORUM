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
    internal class SolicitudAyudaReservaTypeBuilder : IEntityTypeConfiguration<SolicitudAyudaReservaModel>
    {
        public void Configure(EntityTypeBuilder<SolicitudAyudaReservaModel> builder)
        {
            builder.HasKey(e => e.IDReserva);
            builder.Property(e => e.IDReserva).ValueGeneratedOnAdd();
            builder.Property(e => e.IDDisponibilidad).IsRequired();
            builder.Property(e => e.IDUsuarioAyudante).IsRequired().HasMaxLength(450);
            builder.Property(e => e.Estado).IsRequired();

            builder.HasOne(e => e.Disponibilidad)
                   .WithMany(d => d.Reservas)
                   .HasForeignKey(e => e.IDDisponibilidad)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(e => e.UsuarioAyudante)
                   .WithMany(y=> y.SolicitudesAyudaReserva) // si no tenés nav inversa en Users
                   .HasForeignKey(e => e.IDUsuarioAyudante)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(e => new { e.IDUsuarioAyudante, e.Estado })
                   .HasDatabaseName("IX_SolRes_Ayudante");

            builder.HasIndex(e => e.IDDisponibilidad)
                   .IsUnique()
                   .HasFilter("(\"Estado\" IN (0,1,2))")
                   .HasDatabaseName("UX_SolRes_Disponibilidad_Activa");

            builder.ToTable("SolicitudAyudaReserva");
        }
    }
}
