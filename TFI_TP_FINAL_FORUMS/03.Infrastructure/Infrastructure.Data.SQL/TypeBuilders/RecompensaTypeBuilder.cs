using Core.Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.SQL.TypeBuilders
{
    public class RecompensaTypeBuilder : IEntityTypeConfiguration<RecompensaModel>
    {
        public void Configure(EntityTypeBuilder<RecompensaModel> builder)
        {
            builder.HasKey(r => r.IDRecompensa);
            builder.Property(r => r.IDRecompensa).IsRequired();
            builder.Property(r => r.Nombre).IsRequired().HasMaxLength(100);
            builder.Property(r => r.Descripcion).IsRequired().HasColumnType("TEXT");
            builder.Property(r => r.Tipo).IsRequired().HasMaxLength(50);
            builder.Property(r => r.Valor).IsRequired().HasColumnType("int");
            builder.Property(r => r.CreateDate).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.HasMany(r => r.RecompensasUsuarios)
                .WithOne(ru => ru.Recompensa)
                .HasForeignKey(ru => ru.IDRecompensa);

            builder.ToTable("Recompensas");
        }
    }
}
