using Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.SQL.TypeBuilders
{
    public class DenunciaTypeBuilder : IEntityTypeConfiguration<DenunciaModel>
    {
        public void Configure(EntityTypeBuilder<DenunciaModel> builder)
        {
            builder.HasKey(d => d.IDDenuncia);
            builder.Property(d => d.IDUsuarioReporto).IsRequired().HasMaxLength(450);
            builder.Property(d => d.Motivo).IsRequired().HasMaxLength(200);
            builder.Property(d => d.Detalle).HasMaxLength(2000).IsRequired(false);
            builder.Property(d => d.FechaDenuncia).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(d => d.IDPublicacion).IsRequired(false);
            builder.Property(d => d.IDRespuesta).IsRequired(false);

            builder.HasOne(d => d.Publicacion)
                .WithMany(p => p.Denuncias)
                .HasForeignKey(d => d.IDPublicacion)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.Respuesta)
                .WithMany(r => r.Denuncias)
                .HasForeignKey(d => d.IDRespuesta)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasCheckConstraint(
                "CK_Denuncias_PublicacionOrRespuesta",
                "(([IDPublicacion] IS NOT NULL AND [IDRespuesta] IS NULL) OR ([IDPublicacion] IS NULL AND [IDRespuesta] IS NOT NULL))");

            builder.HasIndex(d => d.IDPublicacion);
            builder.HasIndex(d => d.IDRespuesta);
            builder.HasIndex(d => d.IDUsuarioReporto);

            builder.ToTable("Denuncias");
        }
    }
}
