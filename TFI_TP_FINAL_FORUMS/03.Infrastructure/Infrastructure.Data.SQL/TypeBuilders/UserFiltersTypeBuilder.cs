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
    public class UserFiltersTypeBuilder : IEntityTypeConfiguration<UserFiltersModel>
    {
        public void Configure(EntityTypeBuilder<UserFiltersModel> builder)
        {
            builder.HasKey(x => new { x.IDFilter, x.UserId});

            builder.Property(x => x.Value)
                   .IsRequired()
                   .HasColumnType("nvarchar(max)");

            builder.HasOne(x => x.User)
                   .WithMany(x => x.UserFilters)
                   .HasForeignKey(x => x.UserId);

            builder.HasOne(x => x.Filter)
                   .WithMany(x => x.UserFilters)
                   .HasForeignKey(x => x.IDFilter);

            builder.ToTable("UserFilters");
        }
    }
}
