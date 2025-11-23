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
    public class GroupTypeBuilder : IEntityTypeConfiguration<GroupModel>
    {
        public void Configure(EntityTypeBuilder<GroupModel> builder)
        {
            builder.HasKey(x => x.IDGroup);

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasColumnType("varchar(50)");

            builder.Property(x => x.Description)
                   .IsRequired()
                   .HasColumnType("varchar(250)");

            builder.Ignore(x => x.UpdateDate);

            builder.ToTable("Group");
        }
    }
}
