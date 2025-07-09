using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Core.Domain.Models;

namespace Infrastructure.Data.SQL.TypeBuilders
{
    public class PublicacionTypeBuilder : IEntityTypeConfiguration<PublicacionModel>
    {
        public void Configure(EntityTypeBuilder<PublicacionModel> builder)
        {
            builder.HasKey(p => p.IDPublicacion);
            builder.Property(p => p.IDPublicacion).IsRequired();
            builder.Property(p => p.IDUsuario).IsRequired().HasMaxLength(450);
            builder.Property(p => p.Titulo).IsRequired().HasMaxLength(250);
            builder.Property(p => p.Contenido).IsRequired();
            builder.Property(p => p.Recompensa).IsRequired();
            builder.Property(p => p.Visitas).IsRequired();
            builder.Property(p => p.Respondida).IsRequired();
            builder.Property(p => p.Cerrada).IsRequired();
            builder.Property(p => p.FechaCreacion).IsRequired().HasDefaultValueSql("getdate()");
            builder.Property(p => p.FechaCierre).IsRequired(false);
            builder.Ignore(p => p.CreateDate);
            builder.ToTable("Publicaciones");
        }
    }
}
