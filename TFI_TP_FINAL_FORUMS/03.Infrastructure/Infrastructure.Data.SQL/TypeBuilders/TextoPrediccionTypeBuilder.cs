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
    public class TextoPrediccionTypeBuilder : IEntityTypeConfiguration<TextoPrediccionModel>
    {
        public void Configure(EntityTypeBuilder<TextoPrediccionModel> builder)
        {
            builder.HasKey(a => a.IDTextoPrediccion);
            builder.Property(a => a.Texto).IsRequired(false).HasColumnType("nvarchar(max)");
            builder.Property(a => a.Etiquetas).IsRequired(false).HasColumnType("nvarchar(max)");

            builder.ToTable("TextoPredicciones");
        }
    }
}
