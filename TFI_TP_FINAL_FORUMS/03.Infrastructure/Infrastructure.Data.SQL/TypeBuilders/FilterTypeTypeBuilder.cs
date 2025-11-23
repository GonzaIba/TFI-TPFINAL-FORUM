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
    public class FilterTypeTypeBuilder : IEntityTypeConfiguration<FilterTypeModel>
    {
        public void Configure(EntityTypeBuilder<FilterTypeModel> builder)
        {
            builder.HasKey(x => x.IDFilterType);

            builder.Property(x => x.Type)
                   .IsRequired()
                   .HasColumnType("varchar(400)");

            builder.ToTable("FilterType");
        }
    }
}
