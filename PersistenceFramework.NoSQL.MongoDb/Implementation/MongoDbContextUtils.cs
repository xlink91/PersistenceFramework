using MongoDB.Driver;
using PersistenceFramework.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PersistenceFramework.NoSQL.MongoDb.Implementation
{
    public static class MongoDbContextUtils
    {
        public static UpdateDefinition<TEntity> CreateUpdateDefinition<TEntity>(
            IList<(string op, (Expression<Func<TEntity, object>> property, object value) updateInfo)> operations)
        {
            UpdateDefinition<TEntity>[] updateDefinitionArray = new UpdateDefinition<TEntity>[operations.Count];
            int idx = 0;
            foreach (var (op, updateInfo) in operations)
            {
                updateDefinitionArray[idx++] = CreateSingleUpdateDefinition<TEntity>(op, 
                                                updateInfo.property,
                                                updateInfo.value);
            }

            return new UpdateDefinitionBuilder<TEntity>()
                        .Combine(updateDefinitionArray);
        }

        public static UpdateDefinition<TEntity> CreateSingleUpdateDefinition<TEntity>(string op,
            Expression<Func<TEntity, object>> property,
            object value)
        {
            switch (op)
            {
                case MongoDbUpdateDefinitions.SET:
                    return new UpdateDefinitionBuilder<TEntity>()
                        .Set(property, value);
                default:
                    throw new UnknownUpdateDefinitionOperationException(op);
            }
        }
    }
}
