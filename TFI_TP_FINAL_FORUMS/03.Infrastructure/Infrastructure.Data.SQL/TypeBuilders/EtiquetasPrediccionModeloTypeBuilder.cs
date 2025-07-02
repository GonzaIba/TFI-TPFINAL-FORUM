using Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.SQL.TypeBuilders
{
    internal class EtiquetasPrediccionModeloTypeBuilder : IEntityTypeConfiguration<EtiquetasPrediccionModeloModel>
    {
        public void Configure(EntityTypeBuilder<EtiquetasPrediccionModeloModel> builder)
        {
            builder.HasKey(ep => ep.Id);
            builder.Ignore(x => x.UpdateDate);

            builder.ToTable("EtiquetasPrediccionModelo");
        }
    }
}
