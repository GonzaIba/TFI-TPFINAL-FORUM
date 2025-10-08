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
    internal class SolicitudAyudaChatParticipanteTypeBuilder : IEntityTypeConfiguration<SolicitudAyudaChatParticipanteModel>
    {
        public void Configure(EntityTypeBuilder<SolicitudAyudaChatParticipanteModel> builder)
        {
            // PK compuesta (IDChat, IDUsuario)
            builder.HasKey(e => new { e.IDChat, e.IDUsuario });

            builder.Property(e => e.IDChat)
                   .IsRequired();

            builder.Property(e => e.IDUsuario)
                   .IsRequired()
                   .HasMaxLength(450);

            builder.Property(e => e.Rol)
                   .IsRequired(); // 0=Solicitante, 1=Ayudante

            // Relación con Chat (sin tocar tu modelo de Chat)
            builder.HasOne(e => e.Chat)
                   .WithMany(y=> y.Participantes) // si luego agregás ICollection<SolicitudChatParticipanteModel> en Chat, cámbialo por .WithMany(c => c.Participantes)
                   .HasForeignKey(e => e.IDChat)
                   .OnDelete(DeleteBehavior.Cascade);

            // Garantiza "máximo un participante por Rol" dentro del mismo chat (0 solicitante y 1 ayudante)
            builder.HasIndex(e => new { e.IDChat, e.Rol })
                   .IsUnique()
                   .HasDatabaseName("UQ_SCP_ChatRol");

            builder.Ignore(x => x.UpdateDate);

            builder.ToTable("SolicitudAyudaChatParticipante");
        }
    }
}
