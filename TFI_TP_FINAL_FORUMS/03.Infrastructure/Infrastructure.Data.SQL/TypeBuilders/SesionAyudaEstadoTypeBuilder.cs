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
    internal class SesionAyudaEstadoTypeBuilder : IEntityTypeConfiguration<SesionAyudaEstadoModel>
    {
        public void Configure(EntityTypeBuilder<SesionAyudaEstadoModel> builder)
        {
            builder.HasKey(e => e.IDEstado);

            builder.Property(e => e.IDEstado)
                   .ValueGeneratedOnAdd();

            builder.Property(e => e.Estado)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasIndex(e => e.Estado)
                   .IsUnique()
                   .HasDatabaseName("UQ_SesionAyudaEstado_Estado");

            builder.ToTable("SesionAyudaEstado");
        }
    }
}
