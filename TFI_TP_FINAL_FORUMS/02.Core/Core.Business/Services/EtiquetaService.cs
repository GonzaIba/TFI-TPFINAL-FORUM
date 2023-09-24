using Core.Contracts.Repositories;
using Core.Contracts.Services;
using Core.Domain.Exceptions;
using Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Business.Services
{
    public class EtiquetaService : GenericService<EtiquetaModel>, IEtiquetaService
    {
        private readonly IUsersService _usersService;
        public EtiquetaService(
            IUnitOfWork unitOfWork,
            IUsersService usersService
            )
        : base(unitOfWork, unitOfWork.GetRepository<IEtiquetaRepository>())
        {
            _usersService = usersService;
        }

        public async Task<IEnumerable<EtiquetaModel>> ObtenerEtiquetasDetalle()
        {
            try
            {
                var result = await _repository.Get(tracking: false, ignoreQueryFilters: true, includeProperties: "EtiquetasPublicaciones,EtiquetasPublicaciones.Publicacion");
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
