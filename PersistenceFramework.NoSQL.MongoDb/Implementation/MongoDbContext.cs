using MongoDB.Driver;
using PersistenceFramework.Abstractions.NoSQL.MongoDb;
using PersistenceFramework.Attributes;
using PersistenceFramework.Exceptions;
using PersistenceFramework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace PersistenceFramework.NoSQL.MongoDb.Implementation
{
    public class MongoDbContext : IMongoDbContext
    {
        private IMongoClient MongoClient { get; set; }
        private IMongoDatabase MongoDatabase { get; set; }
        private string DatabaseName { get; set; }
        
        public MongoDbContext(string databaseName, string url)
            : this(databaseName, new MongoClient("mongodb://" + url))
        {

        }

        public MongoDbContext(string databaseName, IMongoClient mongoClient)
        {
            MongoClient = mongoClient;
            MongoDatabase = MongoClient.GetDatabase(databaseName);
            DatabaseName = databaseName;
            CreateCollections();
        }

        public void Add<TEntity>(TEntity entity)
            where TEntity : class
        {
            IMongoCollection<TEntity> entityCollection = (IMongoCollection<TEntity>)GetCollection(typeof(TEntity));
            entityCollection.WithWriteConcern(WriteConcern.Acknowledged).InsertOne(entity);
        }

        public void Add<TEntity>(IEnumerable<TEntity> entityList) where TEntity : class
        {
            IMongoCollection<TEntity> entityCollection = (IMongoCollection<TEntity>)GetCollection(typeof(TEntity));
            entityCollection.WithWriteConcern(WriteConcern.Acknowledged).InsertMany(entityList);
        }

        public IQueryable<TEntity> GetEntity<TEntity>(Expression<Func<TEntity, bool>> filter)
            where TEntity : class
        {
            IMongoCollection<TEntity> mongoCollection = (IMongoCollection<TEntity>)GetList(typeof(TEntity));
            IQueryable<TEntity> entityCollection = mongoCollection.AsQueryable().Where(filter);
            return entityCollection;
        }

        protected object GetList(Type type)
        {
            Type DbContextType = this.GetType();
            foreach (PropertyInfo propertyInfo in DbContextType.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (propertyInfo.PropertyType.Name == typeof(IMongoCollection<>).Name && propertyInfo.PropertyType.GenericTypeArguments.First().Name == type.Name)
                    return propertyInfo.GetValue(this);
            }
            throw new NotDeclaredEntityException($"{type.Name} are not declare in context as IMongoCollection<{type.Name}>",
                new Exception("You most declare the entity as private IMongoCollection<> property"));
        }

        protected object GetCollection(Type type)
        {
            Type DbContextType = this.GetType();
            foreach (PropertyInfo propertyInfo in DbContextType.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (propertyInfo.PropertyType.Name == typeof(IMongoCollection<>).Name && propertyInfo.PropertyType.GenericTypeArguments.First().Name == type.Name)
                    return propertyInfo.GetValue(this);
            }
            throw new NotDeclaredEntityException($"{type.Name} are not declare in context as IMongoCollection<{type.Name}>",
                new Exception("You most declare the entity as private IMongoCollection<> property"));
        }

        private void CreateCollections()
        {
            HashSet<string> collectionsName = new HashSet<string>();
            IAsyncCursor<string> collectionNameCursor = MongoDatabase.ListCollectionNames();

            while(collectionNameCursor.MoveNext())
            {
                foreach (string colName in collectionNameCursor.Current)
                {
                    collectionsName.Add(colName);
                }
            }

            foreach (PropertyInfo prop in GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (prop.PropertyType.Name != typeof(IMongoCollection<>).Name)
                    throw new Exception($"Property {prop.Name} is not {typeof(IMongoCollection<>).Name}<> type");
                if (!collectionsName.Contains(prop.Name))
                    MongoDatabase.CreateCollection(prop.Name);

                object mongoCollection = typeof(IMongoDatabase).GetMethod(nameof(MongoDatabase.GetCollection)).MakeGenericMethod(prop.PropertyType.GenericTypeArguments.First()).Invoke(MongoDatabase, new[] { prop.Name, null });
                prop.SetValue(this, mongoCollection);
            }
        }

        public void Update<TEntity>(TEntity entity)
            where TEntity : class
        {
            IMongoCollection<TEntity> mongoCollection = (IMongoCollection<TEntity>)GetCollection(typeof(TEntity));
            mongoCollection.WithWriteConcern(WriteConcern.Acknowledged).ReplaceOne(DynamicLambdaBuilder.GetIdLE(entity), entity);
        }

        public void Update<TEntity>(Expression<Func<TEntity, bool>> cond, TEntity entity)
            where TEntity : class
        {
            IMongoCollection<TEntity> mongoCollection =
                (IMongoCollection<TEntity>)GetCollection(typeof(TEntity));
            mongoCollection.WithWriteConcern(WriteConcern.Acknowledged).
                ReplaceOne(cond, entity);
        }

        public void Remove<TEntity>(TEntity entity)
            where TEntity : class
        {
            IMongoCollection<TEntity> mongoCollection = (IMongoCollection<TEntity>)GetCollection(typeof(TEntity));
            mongoCollection.WithWriteConcern(WriteConcern.Acknowledged).DeleteOne(DynamicLambdaBuilder.GetIdLE(entity, typeof(PersistenceIdAttribute)));
        }

        public void Remove<TEntity>(Expression<Func<TEntity, bool>> cond)
            where TEntity : class
        {
            IMongoCollection<TEntity> mongoCollection = (IMongoCollection<TEntity>)GetCollection(typeof(TEntity));
            mongoCollection.DeleteMany(cond);
        }

        public void RemoveDb()
        {
            MongoClient.DropDatabase(DatabaseName);
        }

        public IQueryable<TEntity> AsQueryable<TEntity>()
            where TEntity : class
        {
            IMongoCollection<TEntity> mongoCollection = (IMongoCollection<TEntity>)GetCollection(typeof(TEntity));
            return mongoCollection.AsQueryable();
        }

        public IAggregateFluent<TEntity> Aggregate<TEntity>(AggregateOptions options = default)
        {
            return ((IMongoCollection<TEntity>)GetList(typeof(TEntity))).Aggregate(options);
        }

        public IAsyncCursor<TResult> Aggregate<TEntity, TResult>(PipelineDefinition<TEntity, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default)
        {
            return ((IMongoCollection<TEntity>)GetList(typeof(TEntity))).Aggregate(pipeline, options, cancellationToken);
        }
    }
}
