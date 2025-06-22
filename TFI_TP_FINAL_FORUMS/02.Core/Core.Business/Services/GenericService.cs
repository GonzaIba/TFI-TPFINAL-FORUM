using Core.Contracts.Repositories;
using Core.Contracts.Services;
using Core.Contracts.UoW;
using System.Linq.Expressions;

namespace Core.Business.Services
{
    public abstract class GenericService<T> : IGenericService<T> where T : class
    {
        protected readonly IUnitOfWorkBase _unitOfWork;
        protected readonly IGenericRepository<T> _repository;

        public GenericService(IUnitOfWorkBase unitOfWork,
            IGenericRepository<T> repository)
        {
            this._unitOfWork = unitOfWork;
            this._repository = repository;
        }

        public virtual Task CreateAsync(T entity)
        {
            _repository.Insert(entity);
            return _unitOfWork.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(T entity)
        {
            await _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(object id)
        {
            T entity = await _repository.GetByIdAsync(id);
            await _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public virtual async Task<T> GetByIdAsync(object id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public virtual Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.Run(() => _repository.TableNoTracking.AsEnumerable<T>());
        }

        public virtual async Task<IEnumerable<T>> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "", bool ignoreQueryFilters = false, bool tracking = false)
        {
            var result = await _repository.Get(filter, orderBy, includeProperties, ignoreQueryFilters, tracking);
            return result;
        }

        protected async Task<T> GetByIdAsync(Expression<Func<T, bool>> func)
        {
            var result = (await _repository.Get(func)).FirstOrDefault();

            return result;
        }

        public virtual async Task<IEnumerable<T>> GetPagedElements<S>(
            int pageIndex,
            int pageCount,
            Expression<Func<T, S>> orderByExpression,
            bool ascending = true,
            Expression<Func<T, bool>> filter = null)
        {
            return await _repository.GetPagedElements(pageIndex, pageCount, orderByExpression, ascending, filter);
        }

        public async Task<bool> CreateAsync(IEnumerable<T> entities)
        {
            var result = true;
            try
            {
                await _repository.Insert(entities);
                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                result = false;
            }
            return result;
        }
    }
}
