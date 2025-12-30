using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace My_Store.Application.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> Query(bool asNoTracking = true);

        /// <summary>Flexible fetch: filter, include, ordering, paging</summary>
        Task<IReadOnlyList<T>> GetAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string[]? includeProperties = null,
            int? skip = null,
            int? take = null,
            bool asNoTracking = true,
            CancellationToken ct = default);

        Task<T?> GetByIdAsync(object id, CancellationToken ct = default);
        Task<T> AddAsync(T entity, CancellationToken ct = default);
        Task UpdateAsync(T entity); 
        Task DeleteAsync(T entity);
    }
}
