using Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.SQL.TypeBuilders
{
    internal class SolicitudAyudaChatMensajeLecturaTypeBuilder : IEntityTypeConfiguration<SolicitudAyudaChatMensajeLecturaModel>
    {
        public void Configure(EntityTypeBuilder<SolicitudAyudaChatMensajeLecturaModel> builder)
        {
            builder.HasKey(e => new { e.IDMensaje, e.IDUsuario });

            builder.Property(e => e.IDUsuario).IsRequired().HasMaxLength(450);

            builder.HasOne(e => e.Mensaje)
                   .WithMany(m => m.Lecturas)
                   .HasForeignKey(e => e.IDMensaje)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => e.IDUsuario).HasDatabaseName("IX_SAChatMsgRead_User");

            builder.ToTable("SolicitudAyudaChatMensajeLectura");
        }
    }
}

