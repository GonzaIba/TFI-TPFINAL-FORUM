using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Models;

namespace Infrastructure.Data.SQL.TypeBuilders
{
    public class RecompensaUsuarioTypeBuilder : IEntityTypeConfiguration<RecompensaUsuarioModel>
    {
        public void Configure(EntityTypeBuilder<RecompensaUsuarioModel> builder)
        {
            builder.HasKey(ru => ru.IDRecompensa);
            builder.Property(ru => ru.IDRecompensa).IsRequired();
            builder.Property(ru => ru.IDUsuario).IsRequired().HasMaxLength(450);
            builder.Property(ru => ru.IDRespuesta).IsRequired();
            builder.Property(ru => ru.FechaObtencion).IsRequired().HasDefaultValueSql("GETDATE()");
            builder.Property(ru => ru.CantidadRecompensa).IsRequired();

            builder.HasOne(ru => ru.Respuesta)
                .WithMany(r => r.RecompensasUsuario)
                .HasForeignKey(ru => ru.IDRespuesta);

            builder.HasOne(um => um.Usuario)
                .WithMany(m => m.RecompensasUsuarios)
                .HasForeignKey(um => um.IDUsuario);

            builder.ToTable("RecompensasUsuario");
        }
    }
}
