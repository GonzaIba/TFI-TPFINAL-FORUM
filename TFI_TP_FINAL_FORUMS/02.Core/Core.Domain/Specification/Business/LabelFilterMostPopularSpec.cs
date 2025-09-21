using Core.Domain.Models;
using System.Linq.Expressions;

namespace Core.Domain.Specification.Business
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    public class LabelFilterMostPopularSpec : Specification<EtiquetaModel>
    {
        // Si querés limitar por ventana (p.ej., últimos 30 días), calculamos _sinceUtc;
        // si no, queda en null y se mide "all-time".
        private readonly DateTime? _sinceUtc;

        /// <param name="lastNDays">
        ///   Cantidad de días hacia atrás para considerar popularidad.
        ///   Ej: 30 = últimos 30 días; null o <= 0 = all-time.
        /// </param>
        /// <param name="nowUtc">Para tests (inyectar "ahora").</param>
        public LabelFilterMostPopularSpec(int? lastNDays = null, DateTime? nowUtc = null)
        {
            if (lastNDays.HasValue && lastNDays.Value > 0)
            {
                var now = nowUtc ?? DateTime.UtcNow;
                _sinceUtc = now.AddDays(-lastNDays.Value);
            }
        }

        // FILTRO: al menos 1 uso (en la ventana si existe, o all-time si no)
        public override Expression<Func<EtiquetaModel, bool>> ToExpression()
        {
            if (_sinceUtc.HasValue)
            {
                var since = _sinceUtc.Value;
                return e => e.EtiquetasPublicaciones
                             .Any(ep => ep.Publicacion.FechaCreacion >= since);
            }

            return e => e.EtiquetasPublicaciones.Any();
        }

        // ORDEN: cantidad de usos (en la ventana si existe, o total si no)
        public Expression<Func<EtiquetaModel, int>> OrderByCountExpr()
        {
            if (_sinceUtc.HasValue)
            {
                var since = _sinceUtc.Value;
                return e => e.EtiquetasPublicaciones
                             .Count(ep => ep.Publicacion.FechaCreacion >= since);
            }

            return e => e.EtiquetasPublicaciones.Count();
        }

        // (Opcional) empate: por nombre
        public Expression<Func<EtiquetaModel, string>> ThenByNameExpr()
            => e => e.NombreEtiqueta;
    }

}
