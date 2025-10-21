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
    internal class TerminosCondicionesTypeBuilder : IEntityTypeConfiguration<TerminosCondicionesModel>
    {
        public void Configure(EntityTypeBuilder<TerminosCondicionesModel> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Nombre)
                   .IsRequired()
                   .HasMaxLength(450);

            builder.Property(e => e.Descripcion)
                   .IsRequired()
                   .HasMaxLength(5000);

            builder.Property(e => e.Version)
                   .IsRequired();

            builder.Ignore(x => x.UpdateDate);

            builder.ToTable("TerminosCondiciones");
        }
    }
}
