using Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.SQL.TypeBuilders
{
    internal class SolicitudAyudaChatMensajeTypeBuilder : IEntityTypeConfiguration<SolicitudAyudaChatMensajeModel>
    {
        public void Configure(EntityTypeBuilder<SolicitudAyudaChatMensajeModel> builder)
        {
            builder.HasKey(e => e.IDMensaje);
            builder.Property(e => e.IDMensaje).ValueGeneratedOnAdd();

            builder.Property(e => e.IDChat).IsRequired();
            builder.Property(e => e.IDUsuario).IsRequired().HasMaxLength(450);
            builder.Property(e => e.Mensaje).IsRequired().HasMaxLength(5000);

            builder.HasOne(e => e.Chat)
                   .WithMany(c => c.Mensajes)
                   .HasForeignKey(e => e.IDChat)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => e.IDChat).HasDatabaseName("IX_SAChatMsg_Chat");
            builder.HasIndex(e => new { e.IDChat, e.CreateDate }).HasDatabaseName("IX_SAChatMsg_ChatDate");

            builder.Ignore(x => x.UpdateDate);

            builder.ToTable("SolicitudAyudaChatMensaje");
        }
    }
}

