using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rmis.Persistence.Abstract;
using Z.EntityFramework.Plus;

namespace Rmis.Persistence
{
    public class EfRepository<T> : IRmisRepository<T> where T : class
    {
        private readonly DbContext dbContext;
        private readonly DbSet<T> dbSet;

        public IQueryable<T> Query { get; private set; }

        public EfRepository(DbContext dbContext, bool asNoTracking)
        {
            this.dbContext = dbContext;
            dbSet = dbContext.Set<T>();
            Query = asNoTracking ? dbSet.AsNoTracking() : dbSet.AsQueryable();
        }

        private EfRepository(DbContext dbContext, IQueryable<T> query)
        {
            this.dbContext = dbContext;
            Query = query;
        }

        #region IQuerable

        public Type ElementType => Query.ElementType;

        public Expression Expression => Query.Expression;

        public IQueryProvider Provider => Query.Provider;

        public IEnumerator<T> GetEnumerator()
        {
            return Query.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Query.GetEnumerator();
        }

        #endregion

        #region IRmisRepository

        public IRmisRepository<T> Include<TProperty>(Expression<Func<T, TProperty>> navigationPropertyPath)
        {
            var query = Query.Include(navigationPropertyPath);
            return new EfRepository<T>(dbContext, query);
        }

        IRmisRepository<T> IRmisRepository<T>.Include<TProperty>(Expression<Func<T, TProperty>> navigationPropertyPath)
        {
            return Include(navigationPropertyPath) as IRmisRepository<T>;
        }        

        public IRmisRepository<T> Where(Expression<Func<T, bool>> predicate)
        {
            var query = Query.Where(predicate);
            return new EfRepository<T>(dbContext, query);
        }

        public IRmisRepository<T> Contains(T item)
        {
            var query = Query.Contains(item);
            return new EfRepository<T>(dbContext, query);
        }

        IRmisRepository<T> IRmisRepository<T>.Where(Expression<Func<T, bool>> predicate)
        {
            return Where(predicate) as IRmisRepository<T>;
        }

        IRmisRepository<T> IRmisRepository<T>.Contains(T predicate)
        {
            return Contains(predicate) as IRmisRepository<T>;
        }

        public IRmisRepository<TResult> Select<TResult>(Expression<Func<T, TResult>> selector) where TResult : class
        {
            var query = Query.Select(selector);
            return new EfRepository<TResult>(dbContext, query);
        }

        IRmisRepository<TResult> IRmisRepository<T>.Select<TResult>(Expression<Func<T, TResult>> selector)
        {
            return Select(selector) as IRmisRepository<TResult>;
        }

        public IRmisRepository<TResult> SelectMany<TResult>(Expression<Func<T, IEnumerable<TResult>>> selector) where TResult : class
        {
            var query = Query.SelectMany(selector);
            return new EfRepository<TResult>(dbContext, query);
        }

        IRmisRepository<TResult> IRmisRepository<T>.SelectMany<TResult>(Expression<Func<T, IEnumerable<TResult>>> selector)
        {
            return SelectMany(selector) as IRmisRepository<TResult>;
        }

        public IRmisRepository<T> Skip(int count)
        {
            var query = Query.Skip(count);
            return new EfRepository<T>(dbContext, query);
        }

        IRmisRepository<T> IRmisRepository<T>.Skip(int count)
        {
            return Skip(count) as IRmisRepository<T>;
        }

        public IRmisRepository<T> Take(int count)
        {
            var query = Query.Take(count);
            return new EfRepository<T>(dbContext, query);
        }

        IRmisRepository<T> IRmisRepository<T>.Take(int count)
        {
            return Take(count) as IRmisRepository<T>;
        }

        public IRmisRepository<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            var query = Query.OrderBy(keySelector);
            return new EfRepository<T>(dbContext, query);
        }

        IRmisRepository<T> IRmisRepository<T>.OrderBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            return OrderBy(keySelector) as IRmisRepository<T>;
        }

        public IRmisRepository<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            var query = Query.OrderByDescending(keySelector);
            return new EfRepository<T>(dbContext, query);
        }

        IRmisRepository<T> IRmisRepository<T>.OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            return OrderByDescending(keySelector) as IRmisRepository<T>;
        }

        public async Task<IList<T>> ToListAsync(CancellationToken cancellationToken = default)
        {
            var result = await Query.ToListAsync(cancellationToken);
            return result;
        }

        public async Task<T> SingleAsync(CancellationToken cancellationToken = default)
        {
            var result = await Query.SingleAsync(cancellationToken);
            return result;
        }

        public async Task<T> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
        {
            var result = await Query.SingleOrDefaultAsync(cancellationToken);
            return result;
        }

        public async Task<T> FirstAsync(CancellationToken cancellationToken = default)
        {
            var result = await Query.FirstAsync(cancellationToken);
            return result;
        }

        public async Task<T> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
        {
            var result = await Query.FirstOrDefaultAsync(cancellationToken);
            return result;
        }

        public Task<bool> AnyAsync(CancellationToken cancellationToken = default)
        {
            return Query.AnyAsync(cancellationToken);
        }

        public Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return Query.CountAsync(cancellationToken);
        }

        #endregion

        #region IRmisRepository

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public void AddRange(params T[] entities)
        {
            dbSet.AddRange(entities);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            dbSet.AddRange(entities);
        }

        public void Update(T entity)
        {
            var entry = dbContext.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                dbSet.Attach(entity);
                entry = dbContext.Entry(entity);
            }
            entry.State = EntityState.Modified;
        }

        public void Update(Expression<Func<T, T>> updateFactory)
        {
            Query.Update(updateFactory);
        }

        public Task UpdateAsync(Expression<Func<T, T>> updateFactory, CancellationToken cancellationToken = default)
        {
            return Query.UpdateAsync(updateFactory, cancellationToken);
        }

        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        public void Delete(Expression<Func<T, bool>> predicate = default)
        {
            var query = predicate == null ? this : dbSet.Where(predicate);
            query.Delete();
        }

        public Task DeleteAsync(Expression<Func<T, bool>> predicate = default, CancellationToken cancellationToken = default)
        {
            var query = predicate == null ? this : dbSet.Where(predicate);
            return query.DeleteAsync(cancellationToken);
        }

        #endregion

    }
}