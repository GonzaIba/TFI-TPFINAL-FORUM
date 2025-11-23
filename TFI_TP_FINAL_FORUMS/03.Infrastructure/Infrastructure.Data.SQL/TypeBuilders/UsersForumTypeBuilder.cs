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
    public class UsersForumTypeBuilder : IEntityTypeConfiguration<UsersForumModel>
    {
        public void Configure(EntityTypeBuilder<UsersForumModel> builder)
        {
            builder.HasKey(x => x.IdUser);

            builder.Property(x => x.ShortDescriptionForum)
                    .HasColumnType("varchar(100)");

            builder.Property(x => x.LongDescriptionForum)
                   .HasColumnType("varchar(500)");

            builder.Property(x => x.ImageForum)
                   .HasColumnType("text");

            builder.HasOne(x => x.User)
                   .WithOne(z => z.UsersForum)
                   .HasForeignKey<UsersForumModel>(f => f.IdUser);

            builder.ToTable("UsersForum");
        }
    }
}
