using Core.Domain.Models;
using System.Linq.Expressions;

namespace Core.Domain.Specification.Business
{
    public class LabelFilterLatestSpec : Specification<EtiquetaModel>
    {
        private readonly DateTime _sinceUtc;

        // Podés inyectar "ahora" en tests si querés.
        public LabelFilterLatestSpec(DateTime? nowUtc = null)
        {
            var now = nowUtc ?? DateTime.UtcNow;
            _sinceUtc = now.AddDays(-7);
        }

        // FILTRO: etiquetas con al menos 1 uso en últimos 7 días
        public override Expression<Func<EtiquetaModel, bool>> ToExpression()
            => e => e.EtiquetasPublicaciones
                      .Any(ep => ep.Publicacion.FechaCreacion >= _sinceUtc);

        // ORDEN: cantidad de usos en últimos 7 días
        public Expression<Func<EtiquetaModel, int>> OrderByCountExpr()
            => e => e.EtiquetasPublicaciones
                      .Count(ep => ep.Publicacion.FechaCreacion >= _sinceUtc);

        // (Opcional) empate: por nombre
        public Expression<Func<EtiquetaModel, string>> ThenByNameExpr()
            => e => e.NombreEtiqueta;
    }
}
