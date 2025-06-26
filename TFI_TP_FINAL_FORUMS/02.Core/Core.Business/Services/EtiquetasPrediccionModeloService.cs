using Core.Contracts.Repositories;
using Core.Contracts.Services;
using Core.Contracts.UoW;
using Core.Domain.Models;
using Infrastructure.ML.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Business.Services
{
    public class EtiquetasPrediccionModeloService : GenericService<EtiquetasPrediccionModeloModel>, IEtiquetasPrediccionModeloService
    {
        private readonly IUnitOfWorkForum _unitOfWork;
        private readonly IEtiquetasPrediccionModeloRepository _repository;
        private readonly ITextoPrediccionRepositoryML _textoPrediccionRepositoryML;

        public EtiquetasPrediccionModeloService(
            IUnitOfWorkForum unitOfWork,
            IEtiquetasPrediccionModeloRepository repository,
            ITextoPrediccionRepositoryML textoPrediccionRepositoryML)
            : base(unitOfWork, repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
            _textoPrediccionRepositoryML = textoPrediccionRepositoryML;
        }

        public async Task TrainAndSaveLabelsAsync()
        {
            var data = await _repository.Get();
            if (data == null || !data.Any())
                throw new InvalidOperationException("No data available for training.");

            var modeloBytes = _textoPrediccionRepositoryML.TrainAndSaveAsync(data.ToList());
            // 4) Guarda los bytes en la tabla
            var entidad = new EtiquetasPrediccionModeloModel
            {
                ModelData = modeloBytes,
                CreatedOn = DateTime.UtcNow
            };
            await _repository.Insert(entidad);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
