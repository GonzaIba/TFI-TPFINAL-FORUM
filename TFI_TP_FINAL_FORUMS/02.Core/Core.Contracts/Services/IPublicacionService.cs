using Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Contracts.Services
{
    public interface IPublicacionService : IGenericService<PublicacionModel>
    {
        Task<bool> CrearPublicacion(string userId,PublicacionModel publicacion);
        Task<IEnumerable<PublicacionModel>> ObtenerPublicaciones();
        Task<IEnumerable<string>> PredecirEtiquetas(string texto);
        Task<IEnumerable<PublicacionModel>> ObtenerPublicacionesPorFiltro(string texto);
    }
}
