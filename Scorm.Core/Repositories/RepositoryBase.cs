using Microsoft.EntityFrameworkCore;
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
    public class RepositoryBase<TEntity, TContext> : IEntityRepository<TEntity>,IAsyncEntityRepository<TEntity>
        where TEntity : class, IEntity, new()
        where TContext : DbContext
    {

        protected TContext Context { get; }
        public RepositoryBase(TContext context)
        {
            Context = context;
        }

        public IQueryable<TEntity> Query()
        {
            return Context.Set<TEntity>();
        }
        public TEntity Add(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Added;
            Context.SaveChangesAsync();
            return entity;
        }

        public TEntity Delete(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Deleted;
            Context.SaveChanges();
            return entity;
        }

        public TEntity? Get(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().FirstOrDefault(predicate);
        }

        public TEntity Update(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            Context.SaveChanges();
            return entity;
        }

        public IPaginate<TEntity> GetList(
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            int index = 0, int size = 10, bool enableTracking = true)
        {
            IQueryable<TEntity> queryable = Query();
            if (!enableTracking) queryable = queryable.AsNoTracking();
            if (include != null) queryable = include(queryable);
            if (predicate != null) queryable = queryable.Where(predicate);
            if (orderBy != null)
                return orderBy(queryable).ToPaginate(index, size);
            return queryable.ToPaginate(index, size);
        }

        public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Context.Set<TEntity>().FirstOrDefaultAsync(predicate);
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Added;
            await Context.SaveChangesAsync();
            return entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            await Context.SaveChangesAsync();
            return entity;
        }

        public async Task<TEntity> DeleteAsync(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Deleted;
            await Context.SaveChangesAsync();
            return entity;
        }

        public async Task<IPaginate<TEntity>> GetListAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            int index = 0, int size = 10, bool enableTracking = true, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> queryable = Query();
            if (!enableTracking) queryable = queryable.AsNoTracking();
            if (include != null) queryable = include(queryable);
            if (predicate != null) queryable = queryable.Where(predicate);
            if (orderBy != null)
                return await orderBy(queryable).ToPaginateAsync(index, size, 0, cancellationToken);
            return await queryable.ToPaginateAsync(index, size, 0, cancellationToken);
        }
    }
}
