using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PersistenceFramework.Contract
{
    public interface IDbContext<TKey>
    {
        IEnumerable<TEntity> GetEntity<TEntity>(Expression<Func<TEntity, bool>> cond)
            where TEntity : class;
        void Add<TEntity>(TEntity entity)
            where TEntity : class;
        void Update<TEntity>(TEntity entity)
            where TEntity : class;
        void Remove<TEntity>(TEntity entity)
            where TEntity : class;
    }
}
