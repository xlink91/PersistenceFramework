﻿using MongoDB.Bson;
using PersistenceFramework.Contract;
using PersistenceFramework.Entities.BaseEntityContract;
using PersistenceFramework.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace PersistenceFramework.Mock.NoSQL.MongoDb
{
    public class MockMongoDbContext : IDbContext<ObjectId>
    {
        private bool AutogenerateId { get; set; }

        public MockMongoDbContext(bool autogeneratedId = true)
        {
            AutogenerateId = autogeneratedId;
        }

        public void Add<TEntity>(TEntity entity)
            where TEntity : class, IKeyIdentity<ObjectId>
        {
            ICollection<TEntity> entityCollection = (ICollection<TEntity>)GetCollection(typeof(TEntity));
            if(AutogenerateId)
                entity.Id = ObjectId.GenerateNewId();
            entityCollection.Add(entity);
        }

        public IEnumerable<TEntity> GetEntity<TEntity>(Expression<Func<TEntity, bool>> cond)
            where TEntity : class, IKeyIdentity<ObjectId>
        {
            IEnumerable<TEntity> entityCollection = (IEnumerable<TEntity>)GetList(typeof(TEntity));
            return entityCollection?.Where(cond.Compile());
        }

        protected IEnumerable<object> GetList(Type type)
        {
            Type DbContextType = this.GetType();
            foreach (PropertyInfo propertyInfo in DbContextType.GetProperties())
            {
                if (propertyInfo.PropertyType.GetInterface(typeof(IEnumerable<>).Name) != null && propertyInfo.Name == type.Name)
                    return (IEnumerable<object>)propertyInfo.GetValue(this);
            }
            throw new NotDeclaredEntityException($"{type.Name} are not declare in context as ICollection<{type.Name}>",
                new Exception("You most declare the entity as public ICollection<> property"));
        }

        protected object GetCollection(Type type)
        {
            Type DbContextType = this.GetType();
            foreach (PropertyInfo propertyInfo in DbContextType.GetProperties())
            {
                if (propertyInfo.PropertyType.Name == typeof(ICollection<>).Name && propertyInfo.Name == type.Name)
                    return propertyInfo.GetValue(this);
            }
            throw new NotDeclaredEntityException($"{type.Name} are not declare in context as ICollection<{type.Name}>",
                new Exception("You most declare the entity as public ICollection<> property"));
        }

        protected void UpdateCollection(Type type, object collection)
        {
            PropertyInfo prop = this.GetType().GetProperty(type.Name);
            prop.SetValue(this, collection);
        }

        public void Update<TEntity>(TEntity entity)
            where TEntity : class, IKeyIdentity<ObjectId>
        {
            ICollection<TEntity> collection = (ICollection<TEntity>)GetCollection(typeof(TEntity));
            List<TEntity> nCollection = new List<TEntity>();

            foreach (var _entity in collection)
            {
                if (_entity.Id == entity.Id)
                    continue;
                nCollection.Add(_entity);
            }
            nCollection.Add(entity);
            UpdateCollection(typeof(TEntity), nCollection);
        }

        void IDbContext<ObjectId>.Remove<TEntity>(TEntity entity)
        {
            ICollection<TEntity> collection = (ICollection<TEntity>)GetCollection(typeof(TEntity));
            TEntity entityStored = collection.Where(x => x.Id == entity.Id).SingleOrDefault();
            collection.Remove(entityStored);
        }
    }
}