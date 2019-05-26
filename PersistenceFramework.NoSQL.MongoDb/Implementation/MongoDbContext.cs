﻿using MongoDB.Bson;
using MongoDB.Driver;
using PersistenceFramework.Contract;
using PersistenceFramework.Entities.BaseEntityContract;
using PersistenceFramework.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PersistenceFramework.NoSQL.MongoDb.Implementation
{
    public class MongoDbContext : IDbContext<ObjectId>
    {
        private IMongoClient MongoClient { get; set; }
        private IMongoDatabase MongoDatabase { get; set; }
        private string DatabaseName { get; set; }

        public MongoDbContext(string databaseName, IMongoClient mongoClient)
        {
            MongoClient = mongoClient;
            MongoDatabase = MongoClient.GetDatabase(databaseName);
            DatabaseName = databaseName;
            CreateCollections();
        }

        public void Add<TEntity>(TEntity entity)
            where TEntity : class, IKeyIdentity<ObjectId>
        {
            IMongoCollection<TEntity> entityCollection = (IMongoCollection<TEntity>)GetCollection(typeof(TEntity));
            entityCollection.WithWriteConcern(WriteConcern.Acknowledged).InsertOne(entity);
        }

        public IEnumerable<TEntity> GetEntity<TEntity>(Expression<Func<TEntity, bool>> filter)
            where TEntity : class, IKeyIdentity<ObjectId>
        {
            IMongoCollection<TEntity> mongoCollection = (IMongoCollection<TEntity>)GetList(typeof(TEntity));
            IEnumerable<TEntity> entityCollection = mongoCollection.Find(filter).ToEnumerable();
            return entityCollection;
        }

        protected object GetList(Type type)
        {
            Type DbContextType = this.GetType();
            foreach (PropertyInfo propertyInfo in DbContextType.GetProperties())
            {
                if (propertyInfo.PropertyType.Name == typeof(IMongoCollection<>).Name && propertyInfo.PropertyType.GenericTypeArguments.First().Name == type.Name)
                    return propertyInfo.GetValue(this);
            }
            throw new NotDeclaredEntityException($"{type.Name} are not declare in context as IMongoCollection<{type.Name}>",
                new Exception("You most declare the entity as public IMongoCollection<> property"));
        }

        protected object GetCollection(Type type)
        {
            Type DbContextType = this.GetType();
            foreach (PropertyInfo propertyInfo in DbContextType.GetProperties())
            {
                if (propertyInfo.PropertyType.Name == typeof(IMongoCollection<>).Name && propertyInfo.PropertyType.GenericTypeArguments.First().Name == type.Name)
                    return propertyInfo.GetValue(this);
            }
            throw new NotDeclaredEntityException($"{type.Name} are not declare in context as IMongoCollection<{type.Name}>",
                new Exception("You most declare the entity as public IMongoCollection<> property"));
        }

        private void CreateCollections()
        {
            HashSet<string> collectionsName = new HashSet<string>();
            IAsyncCursor<string> collectionNameCursor = MongoDatabase.ListCollectionNames();

            for (; collectionNameCursor.MoveNext();)
                foreach (string colName in collectionNameCursor.Current)
                    collectionsName.Add(colName);

            foreach (PropertyInfo prop in GetType().GetProperties())
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
            where TEntity : class, IKeyIdentity<ObjectId>
        {
            IMongoCollection<TEntity> mongoCollection = (IMongoCollection<TEntity>)GetCollection(typeof(TEntity));
            mongoCollection.WithWriteConcern(WriteConcern.Acknowledged).ReplaceOne(x => x.Id == entity.Id, entity);
        }

        public void Remove<TEntity>(TEntity entity)
            where TEntity : class, IKeyIdentity<ObjectId>
        {
            IMongoCollection<TEntity> mongoCollection = (IMongoCollection<TEntity>)GetCollection(typeof(TEntity));
            mongoCollection.WithWriteConcern(WriteConcern.Acknowledged).DeleteOne(x => x.Id == entity.Id);
        }

        public void RemoveDb()
        {
            MongoClient.DropDatabase(DatabaseName);
        }
    }
}