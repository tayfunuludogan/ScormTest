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
    public interface IEntityRepository<T>: IQuery<T> where T : class, IEntity, new()
    {
        T? Get(Expression<Func<T, bool>> predicate);
        IPaginate<T> GetList(Expression<Func<T, bool>>? predicate = null,
                     Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                     Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
                     int index = 0, int size = 100,
                     bool enableTracking = true);
        T Add(T entity);
        T Update(T entity);
        T Delete(T entity);
    }
}
