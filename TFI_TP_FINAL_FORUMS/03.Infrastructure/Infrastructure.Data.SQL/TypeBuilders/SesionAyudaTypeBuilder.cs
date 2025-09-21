using Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.SQL.TypeBuilders
{
    internal class SesionAyudaTypeBuilder : IEntityTypeConfiguration<SesionAyudaModel>
    {
        public void Configure(EntityTypeBuilder<SesionAyudaModel> builder)
        {
            builder.HasKey(e => e.IDSesion);
            builder.Property(e => e.IDSesion).ValueGeneratedNever(); // Guid provisto por app

            builder.Property(e => e.Inicio).HasColumnType("datetime2(3)");
            builder.Property(e => e.Fin).HasColumnType("datetime2(3)");
            builder.Property(e => e.IDReserva).IsRequired();
            builder.Property(e => e.Dominio).IsRequired().HasMaxLength(200); //.HasDefaultValue("meet.jit.si");
            builder.Property(e => e.NombreSala).IsRequired().HasMaxLength(200);

            builder.HasOne(e => e.Reserva)
                   .WithOne(r => r.Sesion)
                   .HasForeignKey<SesionAyudaModel>(e => e.IDReserva)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(e => e.IDReserva).HasDatabaseName("IX_SesionAyuda_Reserva");
            builder.HasIndex(e => new { e.Dominio, e.NombreSala })
                   .IsUnique()
                   .HasDatabaseName("UQ_SesionAyuda_Sala");

            builder.ToTable("SesionAyuda");
        }
    }
}
