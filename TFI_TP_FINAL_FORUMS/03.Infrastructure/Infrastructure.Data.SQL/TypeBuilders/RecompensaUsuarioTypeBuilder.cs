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
            builder.HasKey(ru => ru.IDRecompensaUsuario);
            builder.Property(ru => ru.IDRecompensaUsuario).IsRequired();
            builder.Property(ru => ru.IDUsuario).IsRequired().HasMaxLength(450);
            builder.Property(ru => ru.IDRecompensa).IsRequired();
            builder.Property(ru => ru.FechaObtencion).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne(ru => ru.Recompensa)
                .WithMany(r => r.RecompensasUsuarios)
                .HasForeignKey(ru => ru.IDRecompensa);

            builder.Ignore(x => x.UpdateDate);

            builder.ToTable("RecompensasUsuario");
        }
    }
}
