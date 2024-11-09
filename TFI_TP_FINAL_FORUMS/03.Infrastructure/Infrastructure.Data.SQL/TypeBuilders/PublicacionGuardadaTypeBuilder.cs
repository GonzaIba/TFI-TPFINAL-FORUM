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
    public class PublicacionGuardadaTypeBuilder : IEntityTypeConfiguration<PublicacionGuardadaModel>
    {
        public void Configure(EntityTypeBuilder<PublicacionGuardadaModel> builder)
        {
            builder.HasKey(pg => pg.IDPublicacionGuardada);
            builder.Property(pg => pg.IDPublicacionGuardada).IsRequired();
            builder.Property(pg => pg.IDUsuario).IsRequired().HasMaxLength(450);
            builder.Property(pg => pg.IDPublicacion).IsRequired();

            builder.HasOne(pg => pg.Usuario)
                .WithMany(u => u.PublicacionesGuardadas)
                .HasForeignKey(pg => pg.IDUsuario);

            builder.HasOne(pg => pg.Publicacion)
                .WithMany(p => p.PublicacionesGuardadas)
                .HasForeignKey(pg => pg.IDPublicacion);

            builder.ToTable("PublicacionesGuardadas");
        }
    }
}
