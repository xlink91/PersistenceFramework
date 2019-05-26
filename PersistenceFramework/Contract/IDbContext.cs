using PersistenceFramework.Entities.BaseEntityContract;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PersistenceFramework.Contract
{
    public interface IDbContext<TKey>
    {
        IEnumerable<TEntity> GetEntity<TEntity>(Expression<Func<TEntity, bool>> cond)
            where TEntity : class, IKeyIdentity<TKey>;
        void Add<TEntity>(TEntity entity)
            where TEntity : class, IKeyIdentity<TKey>;
        void Update<TEntity>(TEntity entity)
            where TEntity : class, IKeyIdentity<TKey>;
        void Remove<TEntity>(TEntity entity)
            where TEntity : class, IKeyIdentity<TKey>;
    }
}
