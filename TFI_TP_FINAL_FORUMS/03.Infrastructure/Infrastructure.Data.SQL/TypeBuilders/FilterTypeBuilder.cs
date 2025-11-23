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
    public class FilterTypeBuilder : IEntityTypeConfiguration<FilterModel>
    {
        public void Configure(EntityTypeBuilder<FilterModel> builder)
        {
            builder.HasKey(x => x.IDFilter);

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasColumnType("varchar(50)");

            builder.Property(x => x.Description)
                   .IsRequired()
                   .HasColumnType("varchar(250)");

            builder.Property(x => x.Api)
                   .IsRequired()
                   .IsUnicode(false)
                   .HasMaxLength(50);

            builder.Property(x => x.Options)
                   .IsRequired(false)
                   .HasColumnType("varchar(1000)");

            builder.HasOne(x => x.FilterType)
                   .WithMany(z => z.Filters)
                   .HasForeignKey(f => f.IDFilterType);

            builder.Ignore(x => x.UpdateDate);

            builder.ToTable("Filter");
        }
    }
}
