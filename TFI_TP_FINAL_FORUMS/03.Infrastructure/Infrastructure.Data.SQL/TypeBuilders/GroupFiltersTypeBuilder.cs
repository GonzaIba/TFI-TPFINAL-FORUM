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
    public class GroupFiltersTypeBuilder : IEntityTypeConfiguration<GroupFiltersModel>
    {
        public void Configure(EntityTypeBuilder<GroupFiltersModel> builder)
        {
            builder.HasKey(x => new {x.IDGroup,x.IDFilter});

            builder.HasOne(x => x.Group)
                   .WithMany(x => x.GroupFilters)
                   .HasForeignKey(x => x.IDGroup);

            builder.HasOne(x => x.Filter)
                   .WithMany(x => x.GroupFilters)
                   .HasForeignKey(x => x.IDFilter);

            builder.ToTable("GroupFilters");
        }
    }
}