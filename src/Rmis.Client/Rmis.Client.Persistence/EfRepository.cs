using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rmis.Client.Persistence.Abstract;
using Z.EntityFramework.Plus;

namespace Rmis.Client.Persistence
{
    public class EfRepository<T> : IRmisClientRepository<T> where T : class
    {
        private readonly DbContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public IQueryable<T> Query { get; private set; }

        public EfRepository(DbContext dbContext, bool asNoTracking)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
            Query = asNoTracking ? _dbSet.AsNoTracking() : _dbSet.AsQueryable();
        }

        private EfRepository(DbContext dbContext, IQueryable<T> query)
        {
            this._dbContext = dbContext;
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

        #region IRmisClientRepository

        public IRmisClientRepository<T> Include<TProperty>(Expression<Func<T, TProperty>> navigationPropertyPath)
        {
            var query = Query.Include(navigationPropertyPath);
            return new EfRepository<T>(_dbContext, query);
        }

        IRmisClientRepository<T> IRmisClientRepository<T>.Include<TProperty>(Expression<Func<T, TProperty>> navigationPropertyPath)
        {
            return Include(navigationPropertyPath) as IRmisClientRepository<T>;
        }        

        public IRmisClientRepository<T> Where(Expression<Func<T, bool>> predicate)
        {
            var query = Query.Where(predicate);
            return new EfRepository<T>(_dbContext, query);
        }

        public IRmisClientRepository<T> Contains(T item)
        {
            var query = Query.Contains(item);
            return new EfRepository<T>(_dbContext, query);
        }

        IRmisClientRepository<T> IRmisClientRepository<T>.Where(Expression<Func<T, bool>> predicate)
        {
            return Where(predicate) as IRmisClientRepository<T>;
        }

        IRmisClientRepository<T> IRmisClientRepository<T>.Contains(T predicate)
        {
            return Contains(predicate) as IRmisClientRepository<T>;
        }

        public IRmisClientRepository<TResult> Select<TResult>(Expression<Func<T, TResult>> selector) where TResult : class
        {
            var query = Query.Select(selector);
            return new EfRepository<TResult>(_dbContext, query);
        }

        IRmisClientRepository<TResult> IRmisClientRepository<T>.Select<TResult>(Expression<Func<T, TResult>> selector)
        {
            return Select(selector) as IRmisClientRepository<TResult>;
        }

        public IRmisClientRepository<TResult> SelectMany<TResult>(Expression<Func<T, IEnumerable<TResult>>> selector) where TResult : class
        {
            var query = Query.SelectMany(selector);
            return new EfRepository<TResult>(_dbContext, query);
        }

        IRmisClientRepository<TResult> IRmisClientRepository<T>.SelectMany<TResult>(Expression<Func<T, IEnumerable<TResult>>> selector)
        {
            return SelectMany(selector) as IRmisClientRepository<TResult>;
        }

        public IRmisClientRepository<T> Skip(int count)
        {
            var query = Query.Skip(count);
            return new EfRepository<T>(_dbContext, query);
        }

        IRmisClientRepository<T> IRmisClientRepository<T>.Skip(int count)
        {
            return Skip(count) as IRmisClientRepository<T>;
        }

        public IRmisClientRepository<T> Take(int count)
        {
            var query = Query.Take(count);
            return new EfRepository<T>(_dbContext, query);
        }

        IRmisClientRepository<T> IRmisClientRepository<T>.Take(int count)
        {
            return Take(count) as IRmisClientRepository<T>;
        }

        public IRmisClientRepository<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            var query = Query.OrderBy(keySelector);
            return new EfRepository<T>(_dbContext, query);
        }

        IRmisClientRepository<T> IRmisClientRepository<T>.OrderBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            return OrderBy(keySelector) as IRmisClientRepository<T>;
        }

        public IRmisClientRepository<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            var query = Query.OrderByDescending(keySelector);
            return new EfRepository<T>(_dbContext, query);
        }

        IRmisClientRepository<T> IRmisClientRepository<T>.OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            return OrderByDescending(keySelector) as IRmisClientRepository<T>;
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

        #region IRmisClientRepository

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void AddRange(params T[] entities)
        {
            _dbSet.AddRange(entities);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            _dbSet.AddRange(entities);
        }

        public void Update(T entity)
        {
            var entry = _dbContext.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
                entry = _dbContext.Entry(entity);
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
            _dbSet.Remove(entity);
        }

        public int Delete(Expression<Func<T, bool>> predicate = default)
        {
            var query = predicate == null ? this : _dbSet.Where(predicate);
            return query.Delete();
        }

        public Task DeleteAsync(Expression<Func<T, bool>> predicate = default, CancellationToken cancellationToken = default)
        {
            var query = predicate == null ? this : _dbSet.Where(predicate);
            return query.DeleteAsync(cancellationToken);
        }

        #endregion

    }
}