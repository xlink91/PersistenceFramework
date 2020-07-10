using System;
using System.Linq;
using System.Linq.Expressions;

namespace PersistenceFramework.Contract
{
    public interface IDbContext
    {
        IQueryable<TEntity> GetEntity<TEntity>(Expression<Func<TEntity, bool>> cond)
            where TEntity : class;
        void Add<TEntity>(TEntity entity)
            where TEntity : class;
        void Add<TEntity>(IEnumerable<TEntity> entityList)
            where TEntity : class;
        void Update<TEntity>(TEntity entity)
            where TEntity : class;
        void Update<TEntity>(Expression<Func<TEntity, bool>> cond, TEntity entity)
            where TEntity : class;
        void Remove<TEntity>(TEntity entity)
            where TEntity : class;
    }
}
