using Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Specification.Business
{
    /// <summary>
    /// Especificación: Solicitudes de ayuda válidas en el "momento de anclaje".
    /// Incluye las creadas en o antes de anchorUtc y que aún no hayan vencido.
    /// </summary>
    public class RequestHelpValidAtSpec : Specification<SolicitudAyudaModel>
    {
        private readonly DateTime _anchorUtc;

        /// <param name="anchorUtc">
        /// Momento de referencia para validar. Si no se pasa, se toma UtcNow.
        /// </param>
        public RequestHelpValidAtSpec(DateTime? anchorUtc = null)
        {
            _anchorUtc = anchorUtc ?? DateTime.UtcNow;
        }

        public override Expression<Func<SolicitudAyudaModel, bool>> ToExpression()
            => z => z.CreateDate <= _anchorUtc
                  && z.FechaVencimiento >= DateTime.UtcNow;
    }
}
