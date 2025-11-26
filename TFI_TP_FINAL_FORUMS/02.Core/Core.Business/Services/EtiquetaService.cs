using Core.Contracts.Repositories;
using Core.Contracts.Services;
using Core.Contracts.UoW;
using Core.Domain.Enum;
using Core.Domain.GenericEntityClass;
using Core.Domain.Models;
using Core.Domain.Specification.Business;

namespace Core.Business.Services
{
    public class EtiquetaService : GenericService<EtiquetaModel>, IEtiquetaService
    {
        private readonly IUsersService _usersService;
        public EtiquetaService(
            IUnitOfWorkForum unitOfWork,
            IUsersService usersService
            )
        : base(unitOfWork, unitOfWork.GetRepository<IEtiquetaRepository>())
        {
            _usersService = usersService;
        }

        public async Task<bool> CreateLabel(string nombreEtiqueta)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            EtiquetaModel etiquetaModel = new();
            etiquetaModel.NombreEtiqueta = nombreEtiqueta;
            etiquetaModel.CreateDate = DateTime.UtcNow;
            await _repository.Insert(etiquetaModel);
            await _unitOfWork.SaveChangesAsync();
            var r = etiquetaModel.IDEtiqueta;
            await transaction.CommitAsync();
            return etiquetaModel.IDEtiqueta > 0;
        }

        public async Task<PaginatedList<EtiquetaModel>> GetLabelsByFilter(LabelFiltersEnum filter, int pageIndex, int pageCount)
        {
            try
            {
                switch (filter)
                {
                    case LabelFiltersEnum.MostPopular:
                        var specMostPopular = new LabelFilterMostPopularSpec();
                        return await _repository.GetPagedElements(
                            pageIndex, pageCount,
                            orderBy: q => q.Where(specMostPopular.ToExpression())
                                           .OrderByDescending(specMostPopular.OrderByCountExpr())
                                           .ThenBy(specMostPopular.ThenByNameExpr()),
                            includeProperties: "EtiquetasPublicaciones,EtiquetasPublicaciones.Publicacion",
                            tracking: false
                        );

                    case LabelFiltersEnum.Latest:
                        var specLatest = new LabelFilterLatestSpec();
                        return await _repository.GetPagedElements(
                            pageIndex,
                            pageCount,
                            orderBy: q => q.Where(specLatest.ToExpression())
                                           .OrderByDescending(specLatest.OrderByCountExpr())
                                           .ThenBy(specLatest.ThenByNameExpr()),
                            filter: specLatest, // filtra a las que tienen al menos 1 en 7 días
                            includeProperties: "EtiquetasPublicaciones,EtiquetasPublicaciones.Publicacion",
                            tracking: false
                        );
                    case LabelFiltersEnum.Alphabetical_AZ:
                        return await _repository.GetPagedElements(
                            pageIndex,
                            pageCount,
                            orderByExpression: p => p.NombreEtiqueta,
                            ascending: true,
                            includeProperties: "EtiquetasPublicaciones,EtiquetasPublicaciones.Publicacion",
                            tracking: false
                        );
                    case LabelFiltersEnum.Alphabetical_ZA:
                        return await _repository.GetPagedElements(
                            pageIndex,
                            pageCount,
                            orderByExpression: p => p.NombreEtiqueta,
                            ascending: false,
                            includeProperties: "EtiquetasPublicaciones,EtiquetasPublicaciones.Publicacion",
                            tracking: false
                        );
                    default: throw new NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<PaginatedList<EtiquetaModel>> GetLabelsByName(string rawQuery, int pageIndex, int pageCount)
        {
            try
            {
                var paged = await _repository.GetPagedElements(
                    pageIndex,
                    pageCount,
                    orderByExpression: p => p.NombreEtiqueta,
                    ascending: false,
                    includeProperties: "EtiquetasPublicaciones,EtiquetasPublicaciones.Publicacion",
                    tracking: false
                );
                return paged;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
