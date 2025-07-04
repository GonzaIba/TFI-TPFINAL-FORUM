using Core.Domain.GenericEntityClass;
using Core.Domain.Specification;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Contracts.Services
{
    public partial interface IGenericService<T> where T : class
    {
        public Task CreateAsync(T entity);

        public Task<bool> CreateAsync(IEnumerable<T> entities);

        public Task DeleteAsync(object id);

        public Task<IEnumerable<T>> GetAllAsync();

        public Task<T> GetByIdAsync(object id);

        public Task UpdateAsync(T entity);

        public Task<IEnumerable<T>> Get(
        Expression<Func<T, bool>> filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        string includeProperties = "",
        bool ignoreQueryFilters = false,
        bool tracking = true);

        public Task<PaginatedList<T>> GetPagedElements<S>(
        int pageIndex,
        int pageCount,
        Expression<Func<T, S>> orderByExpression,
        bool ascending,
        Specification<T> filter = null);
    }
}