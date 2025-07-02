using Core.Contracts.Repositories;
using Core.Contracts.Services;
using Core.Contracts.UoW;
using Core.Domain.Models;
using Infrastructure.ML.Contracts;

namespace Core.Business.Services
{
    public class EtiquetasPrediccionModeloService : GenericService<EtiquetasPrediccionModeloModel>, IEtiquetasPrediccionModeloService
    {
        private readonly IUnitOfWorkForum _unitOfWork;
        private readonly IEtiquetasPrediccionModeloRepository _repository;
        private readonly ITextoPrediccionRepository _textoPrediccionrepository;
        private readonly ITextoPrediccionRepositoryML _textoPrediccionRepositoryML;

        public EtiquetasPrediccionModeloService(
            IUnitOfWorkForum unitOfWork,
            IEtiquetasPrediccionModeloRepository repository,
            ITextoPrediccionRepository textoPrediccionrepository,
            ITextoPrediccionRepositoryML textoPrediccionRepositoryML)
            : base(unitOfWork, repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
            _textoPrediccionrepository = textoPrediccionrepository;
            _textoPrediccionRepositoryML = textoPrediccionRepositoryML;
        }

        public async Task TrainAndSaveLabelsAsync()
        {
            var data = await _textoPrediccionrepository.Get();

            var modeloBytes = _textoPrediccionRepositoryML.TrainAndSaveAsync(data.ToList());
            // 4) Guarda los bytes en la tabla
            var entidad = new EtiquetasPrediccionModeloModel
            {
                ModelData = modeloBytes
            };
            await _repository.Insert(entidad);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
