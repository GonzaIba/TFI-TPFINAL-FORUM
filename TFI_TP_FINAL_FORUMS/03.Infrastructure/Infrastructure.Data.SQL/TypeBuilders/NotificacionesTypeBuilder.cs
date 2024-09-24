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
    public class NotificacionesTypeBuilder : IEntityTypeConfiguration<NotificacionesModel>
    {
        public void Configure(EntityTypeBuilder<NotificacionesModel> builder)
        {
            builder.HasKey(p => p.IDNotificacion);
            builder.Property(p => p.IDUsuario).IsRequired().HasMaxLength(450);
            builder.Property(p => p.Mensaje).IsRequired().HasMaxLength(5000);
            builder.Property(p => p.FechaNotificacion).IsRequired();
            builder.Property(p => p.Leida).IsRequired();
            builder.Ignore(p => p.CreateDate);

            builder.ToTable("Notificaciones");
        }
    }
}
