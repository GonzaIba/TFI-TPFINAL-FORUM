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
    public class EtiquetaPublicacionTypeBuilder : IEntityTypeConfiguration<EtiquetaPublicacionModel>
    {
        public void Configure(EntityTypeBuilder<EtiquetaPublicacionModel> builder)
        {
            builder.HasKey(ep => ep.IDEtiquetaPublicacion);
            builder.Property(ep => ep.IDEtiquetaPublicacion).IsRequired();
            builder.Property(ep => ep.IDPublicacion).IsRequired();
            builder.Property(ep => ep.IDEtiqueta).IsRequired();

            builder.HasOne(ep => ep.Publicacion)
                .WithMany(p => p.EtiquetasPublicacion)
                .HasForeignKey(ep => ep.IDPublicacion);

            builder.HasOne(ep => ep.Etiqueta)
                .WithMany(e => e.EtiquetasPublicacion)
                .HasForeignKey(ep => ep.IDEtiqueta);
            
            builder.ToTable("EtiquetasPublicacion");
        }
    }
}
