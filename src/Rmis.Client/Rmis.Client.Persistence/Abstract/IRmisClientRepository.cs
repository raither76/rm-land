using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Rmis.Client.Persistence.Abstract
{
    public interface IRmisClientRepository<T> : IQueryable<T> where T : class
    {
        IRmisClientRepository<T> Include<TProperty>(Expression<Func<T, TProperty>> navigationPropertyPath);

        IRmisClientRepository<T> Where(Expression<Func<T, bool>> predicate);

        IRmisClientRepository<TResult> Select<TResult>(Expression<Func<T, TResult>> selector) where TResult : class;

        IRmisClientRepository<TResult> SelectMany<TResult>(Expression<Func<T, IEnumerable<TResult>>> selector) where TResult : class;

        IRmisClientRepository<T> Skip(int count);

        IRmisClientRepository<T> Take(int count);

        IRmisClientRepository<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector);

        IRmisClientRepository<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector);

        void Add(T entity);

        void AddRange(params T[] entities);

        void AddRange(IEnumerable<T> entities);

        void Update(T entity);

        void Update(Expression<Func<T, T>> updateFactory);

        Task UpdateAsync(Expression<Func<T, T>> updateFactory, CancellationToken cancellationToken = default);

        void Delete(T entity);

        int Delete(Expression<Func<T, bool>> predicate = default);

        Task DeleteAsync(Expression<Func<T, bool>> predicate = default, CancellationToken cancellationToken = default);
        
        IRmisClientRepository<T> Contains(T predicate);
        
        IQueryable<T> Query { get; }

        Task<IList<T>> ToListAsync(CancellationToken cancellationToken = default);

        Task<T> SingleAsync(CancellationToken cancellationToken = default);

        Task<T> SingleOrDefaultAsync(CancellationToken cancellationToken = default);

        Task<T> FirstAsync(CancellationToken cancellationToken = default);

        Task<T> FirstOrDefaultAsync(CancellationToken cancellationToken = default);

        Task<bool> AnyAsync(CancellationToken cancellationToken = default);

        Task<int> CountAsync(CancellationToken cancellationToken = default);
    }
}