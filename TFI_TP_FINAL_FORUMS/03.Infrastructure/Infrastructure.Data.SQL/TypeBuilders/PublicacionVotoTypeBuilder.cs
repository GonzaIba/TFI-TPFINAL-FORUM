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
    public class PublicacionVotoTypeBuilder : IEntityTypeConfiguration<PublicacionVotoModel>
    {
        public void Configure(EntityTypeBuilder<PublicacionVotoModel> builder)
        {
            builder.HasKey(p => new { p.IDPublicacion, p.IDUsuario});

            builder.HasOne(pg => pg.Usuario)
                .WithMany(u => u.PublicacionesVotos)
                .HasForeignKey(pg => pg.IDUsuario);

            builder.HasOne(pg => pg.Publicacion)
                .WithMany(p => p.PublicacionesVotos)
                .HasForeignKey(pg => pg.IDPublicacion);

            builder.Ignore(x => x.UpdateDate);
            builder.Ignore(x => x.Active);

            builder.ToTable("PublicacionesVotos");
        }
    }
}
