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
    internal class EtiquetaTypeBuilder : IEntityTypeConfiguration<EtiquetaModel>
    {
        public void Configure(EntityTypeBuilder<EtiquetaModel> builder)
        {
            builder.HasKey(e => e.IDEtiqueta);

            builder.Property(e => e.IDEtiqueta).IsRequired();

            builder.Property(e => e.NombreEtiqueta).IsRequired().HasMaxLength(100);
            builder.ToTable("Etiquetas");
        }
    }
}
