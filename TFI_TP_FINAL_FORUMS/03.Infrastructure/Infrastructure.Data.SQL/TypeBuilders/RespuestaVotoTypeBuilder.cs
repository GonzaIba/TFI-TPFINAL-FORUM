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
    public class RespuestaVotoTypeBuilder : IEntityTypeConfiguration<RespuestaVotoModel>
    {
        public void Configure(EntityTypeBuilder<RespuestaVotoModel> builder)
        {
            builder.HasKey(p => new { p.IDRespuesta, p.IDUsuario });

            builder.HasOne(pg => pg.Usuario)
                .WithMany(u => u.RespuestasVotos)
                .HasForeignKey(pg => pg.IDUsuario);

            builder.HasOne(pg => pg.Respuesta)
                .WithMany(p => p.RespuestasVotos)
                .HasForeignKey(pg => pg.IDRespuesta);

            builder.Ignore(x => x.UpdateDate);
            builder.Ignore(x => x.Active);

            builder.ToTable("RespuestasVotos");
        }
    }
}
