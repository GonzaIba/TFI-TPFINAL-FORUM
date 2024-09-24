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
    public class RespuestaTypeBuilder : IEntityTypeConfiguration<RespuestaModel>
    {
        public void Configure(EntityTypeBuilder<RespuestaModel> builder)
        {
            builder.HasKey(r => r.IDRespuesta);
            builder.Property(r => r.IDRespuesta).IsRequired();
            builder.Property(r => r.IDPublicacion).IsRequired();
            builder.Property(r => r.IDUsuario).IsRequired().HasMaxLength(450);
            builder.Property(r => r.TextoRespuesta).IsRequired();
            builder.Property(r => r.FechaCreacion).IsRequired().HasDefaultValueSql("GETDATE()");
            builder.Property(r => r.RespuestaCorrecta).IsRequired();
            builder.Property(r => r.Votos).IsRequired();

            builder.HasOne(r => r.Publicacion)
                .WithMany(p => p.Respuestas)
                .HasForeignKey(r => r.IDPublicacion)
                .OnDelete(DeleteBehavior.NoAction); // Evitar cascada

            builder.ToTable("Respuestas");
        }
    }
}
