using Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.SQL.TypeBuilders
{
    internal class SolicitudAyudaChatTypeBuilder : IEntityTypeConfiguration<SolicitudAyudaChatModel>
    {
        public void Configure(EntityTypeBuilder<SolicitudAyudaChatModel> builder)
        {
            builder.HasKey(e => e.IDChat);
            builder.Property(e => e.IDChat).ValueGeneratedOnAdd();

            builder.Property(e => e.IDSolicitudAyuda).IsRequired();
            builder.Property(e => e.IDUsuarioAyudante).IsRequired().HasMaxLength(450);

            builder.HasOne(e => e.Solicitud)
                   .WithMany(s => s.Chats)
                   .HasForeignKey(e => e.IDSolicitudAyuda)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => new { e.IDSolicitudAyuda, e.IDUsuarioAyudante })
                   .IsUnique()
                   .HasDatabaseName("UQ_SAChat_SolicitudAyudante");

            builder.HasIndex(e => e.IDUsuarioAyudante)
                   .HasDatabaseName("IX_SAChat_Ayudante");

            builder.Ignore(x => x.UpdateDate);

            builder.ToTable("SolicitudAyudaChat");
        }
    }
}
