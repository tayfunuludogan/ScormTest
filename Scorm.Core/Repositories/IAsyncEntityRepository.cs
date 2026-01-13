using Microsoft.EntityFrameworkCore.Query;
using Scorm.Core.Entities;
using Scorm.Core.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Core.Repositories
{
    public interface IAsyncEntityRepository<T> : IQuery<T> where T : class, IEntity, new()
    {
        Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
        Task<IPaginate<T>> GetListAsync(Expression<Func<T, bool>>? predicate = null,
                                Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
                                int index = 0, int size = 100, bool enableTracking = true,
                                CancellationToken cancellationToken = default);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T> DeleteAsync(T entity);
    }
}
