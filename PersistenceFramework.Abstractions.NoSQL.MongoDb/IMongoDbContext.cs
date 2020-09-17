using MongoDB.Driver;
using PersistenceFramework.Contract;
using System.Threading;

namespace PersistenceFramework.Abstractions.NoSQL.MongoDb
{
    public interface IMongoDbContext : IDbContext
    {
        IMongoClient MongoClient { get; set; }
        IMongoDatabase MongoDatabase { get; set; }
        IAggregateFluent<TEntity> Aggregate<TEntity>(AggregateOptions options = default);
        IAsyncCursor<TResult> Aggregate<TEntity, TResult>(PipelineDefinition<TEntity, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default);
    }
}
