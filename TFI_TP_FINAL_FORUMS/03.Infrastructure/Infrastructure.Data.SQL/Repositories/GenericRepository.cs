using Core.Contracts.Repositories;
using Core.Domain.GenericEntityClass;
using Core.Domain.Specification;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1;
using System.Linq.Expressions;

namespace Infrastructure.Data.SQL.Repositories
{
    public abstract class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        #region Fields

        internal DbContext _context;
        internal DbSet<T> _entities;

        #endregion Fields

        #region Constructor

        public GenericRepository(DbContext context)
        {
            this._context = context;
        }

        #endregion Constructor

        #region Properties

        public virtual IQueryable<T> Table => this.Entities;

        public virtual IQueryable<T> TableNoTracking => this.Entities.AsNoTracking();

        protected virtual DbSet<T> Entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = _context.Set<T>();
                }
                return _entities;
            }
        }

        #endregion Properties

        #region Methods

        public virtual T GetById(object id) => this.Entities?.Find(id);
        
        public async Task<T> GetByIdAsync(object id) => await this.Entities.FindAsync(id);

        public virtual async Task Insert(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await this.Entities.AddAsync(entity);
            await Task.CompletedTask;
        }

        public virtual async Task Insert(IEnumerable<T> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            foreach (T entity in entities)
            {
                await this.Insert(entity);
            }
        }

        public virtual async Task Update(T entity)
        {

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            this._context.Entry(entity).State = EntityState.Modified;
        }

        public virtual async Task Update(IEnumerable<T> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            foreach (T item in entities)
            {
                await this.Update(item);
            }
        }

        public virtual async Task Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            this._context.Entry(entity).State = EntityState.Deleted;

            await Task.CompletedTask;
        }

        public virtual async Task Delete(IEnumerable<T> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            foreach (T entity in entities)
            {
                await this.Delete(entity);
            }
        }

        public IQueryable<T> Query(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string includeProperties = "",
            bool ignoreQueryFilters = false,
            bool tracking = true)
        {
            IQueryable<T> query = tracking ? this.Entities : this.Entities.AsNoTracking();

            if (!tracking) query = query.AsNoTracking();
            if (ignoreQueryFilters) query = query.IgnoreQueryFilters();
            if (filter is not null) query = query.Where(filter);

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                    query = query.Include(includeProp);
            }

            if (orderBy is not null) query = orderBy(query);

            return query;
        }


        public virtual async Task<IEnumerable<TResult>> GetWithGroupBy<TResult>(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IQueryable<TResult>> selector = null,
            string includeProperties = "",
            bool ignoreQueryFilters = false,
            bool tracking = true)
        {
            IQueryable<T> query = tracking ? this.Entities : this.Entities.AsNoTracking();

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (string includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty.Trim());
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (selector != null)
            {
                return await selector(query).ToListAsync();
            }
            else
            {
                return await query.Cast<TResult>().ToListAsync();
            }
        }

        public virtual async Task<IEnumerable<TResult>> GetWithGroupByDos<TResult>(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Expression<Func<T, IGrouping<T, TResult>>> groupBy = null,
            string includeProperties = "",
            bool ignoreQueryFilters = false,
            bool tracking = true)
        {
            IQueryable<T> query = tracking ? this.Entities : this.Entities.AsNoTracking();

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (string includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty.Trim());
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if(groupBy != null)
            {
                query = query.GroupBy(groupBy).SelectMany(x => x);
            }

            return await query.Cast<TResult>().ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "",
            bool ignoreQueryFilters = false,
            bool tracking = true)
        {
            IQueryable<T> query = tracking ? this.Entities : this.Entities.AsNoTracking();

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (string includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty.Trim());
            }

            if (orderBy != null)
            {
                return orderBy(query);
            }
            else
            {
                return query;
            }
        }

        public async Task<T> GetOne(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "",
            bool ignoreQueryFilters = false,
            bool tracking = true)
        {
            var result = await Get(filter, orderBy, includeProperties, ignoreQueryFilters, tracking);
            return result.FirstOrDefault();
        }

        public virtual async Task<PaginatedList<T>> GetPagedElements<S>(
             int pageIndex, int pageCount,
             Expression<Func<T, S>> orderByExpression, bool ascending,
             Specification<T> filter = null, string includeProperties = "", bool tracking = false)
        {
            //Verificar los argumentos para esta consulta
            if (pageIndex < 0)
            {
                throw new ArgumentException(
                    //Resources.Messages.exception_InvalidPageIndex,
                    "pageIndex");
            }

            if (pageCount <= 0)
            {
                throw new ArgumentException(
                    //Resources.Messages.exception_InvalidPageCount,
                    "pageCount");
            }

            if (orderByExpression == null)
            {
                throw new ArgumentNullException(nameof(orderByExpression)
                        //, Resources.Messages.exception_OrderByExpressionCannotBeNull
                        );
            }

            IQueryable<T> query = tracking ? this.Entities : this.Entities.AsNoTracking();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!String.IsNullOrEmpty(includeProperties))
            {
                foreach (string includeProperty in includeProperties.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            int totalPages = (int)Math.Ceiling(query.Count() / (double)pageCount);
            pageIndex = pageIndex == 0 ? 1 : pageIndex;
            return new PaginatedList<T>
            {
                PageIndex = pageIndex,
                PageCount = pageCount,
                TotalCount = query.Count(),
                TotalPages = totalPages == 0 ? 1 : totalPages,
                HasPreviousPage = pageIndex > 1,
                HasNextPage = pageIndex < totalPages,
                List = (ascending)
                            ?
                        query.OrderBy(orderByExpression)
                            .Skip((pageIndex - 1) * pageCount)
                            .Take(pageCount)
                            :
                        query.OrderByDescending(orderByExpression)
                            .Skip((pageIndex - 1) * pageCount)
                            .Take(pageCount)
            };
        }

        public virtual async Task<PaginatedList<T>> GetPagedElements(
            int pageIndex, int pageCount,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
            Specification<T> filter = null, string includeProperties = null,
            bool tracking = false)
        {
            if (pageCount <= 0) throw new ArgumentException(nameof(pageCount));
            if (orderBy is null) throw new ArgumentNullException(nameof(orderBy));
            if (pageIndex <= 0) pageIndex = 1; // 1-based

            IQueryable<T> query = tracking ? this.Entities : this.Entities.AsNoTracking();

            if (filter != null)
                query = query.Where(filter.ToExpression());

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    query = query.Include(includeProperty.Trim());
                query = query.AsSplitQuery();
            }

            var totalCount = await query.CountAsync().ConfigureAwait(false);
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageCount));

            // (opcional) clamp a rango válido para evitar páginas vacías involuntarias
            // pageIndex = Math.Min(pageIndex, totalPages);

            var ordered = orderBy(query);

            var list = await ordered
                .Skip((pageIndex - 1) * pageCount)
                .Take(pageCount)
                .ToListAsync()
                .ConfigureAwait(false);

            return new PaginatedList<T>
            {
                PageIndex = pageIndex,
                PageCount = pageCount,
                TotalCount = totalCount,
                TotalPages = totalPages,
                List = list,
                HasPreviousPage = pageIndex > 1,
                HasNextPage = pageIndex < totalPages
            };
        }


        public virtual async Task CancelChanges(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            this._context.Entry(entity).State = EntityState.Unchanged;
            await Task.CompletedTask;
        }


        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion Methods
    }
}
