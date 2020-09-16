using System;
using System.Collections.Generic;
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
        void Remove<TEntity>(Expression<Func<TEntity, bool>> cond)
            where TEntity : class;
        IQueryable<TEntity> AsQueryable<TEntity>()
            where TEntity: class;
        void AddAsTransaction<TEntity>(IEnumerable<TEntity> entityList)
            where TEntity : class;
        void UpdateAsTransaction<TEntity, TResult>(IEnumerable<TEntity> entityList,
            Expression<Func<TEntity, TResult>> filter)
            where TEntity : class;
        void RemoveAsTransaction<TEntity>(Expression<Func<TEntity, bool>> filter)
            where TEntity : class;
    }
}
