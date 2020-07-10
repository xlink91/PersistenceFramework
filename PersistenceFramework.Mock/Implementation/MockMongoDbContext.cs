﻿using MongoDB.Bson;
using PersistenceFramework.Attributes;
using PersistenceFramework.Contract;
using PersistenceFramework.Exceptions;
using PersistenceFramework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace PersistenceFramework.Mock.NoSQL.MongoDb
{
    public class MockMongoDbContext : IDbContext
    {
        protected bool AutogenerateId { get; set; }

        public MockMongoDbContext(bool autogeneratedId = true)
        {
            AutogenerateId = autogeneratedId;
        }


        public void Add<TEntity>(TEntity entity)
            where TEntity : class
        {
            ICollection<TEntity> entityCollection = (ICollection<TEntity>)GetCollection(typeof(TEntity));
            if (AutogenerateId)
            {
                PropertyInfo keyPropertyInfo = DynamicLambdaBuilder.GetKeyProperty<TEntity>(typeof(PersistenceIdAttribute));
                if(keyPropertyInfo != default)
                {
                    keyPropertyInfo.SetValue(entity, ObjectId.GenerateNewId());
                }
                else
                {
                    entity.GetType().GetProperty("Id").SetValue(entity, ObjectId.GenerateNewId());
                }
            }
            entityCollection.Add(entity);
        }

        public void Add<TEntity>(IEnumerable<TEntity> entityList) where TEntity : class
        {
            PropertyInfo keyPropertyInfo = AutogenerateId ?
                DynamicLambdaBuilder.GetKeyProperty<TEntity>(typeof(PersistenceIdAttribute)) : default;

            ICollection<TEntity> entityCollection = (ICollection<TEntity>)GetCollection(typeof(TEntity));
            foreach (var entity in entityList)
            {
                if (keyPropertyInfo != default)
                {
                    keyPropertyInfo.SetValue(entity, ObjectId.GenerateNewId());
                }
                else
                {
                    entity.GetType().GetProperty("Id").SetValue(entity, ObjectId.GenerateNewId());
                }
                entityCollection.Add(entity);
            } 
        }

        public IQueryable<TEntity> GetEntity<TEntity>(Expression<Func<TEntity, bool>> cond)
            where TEntity : class 
        {
            IQueryable<TEntity> entityCollection = ((ICollection<TEntity>)GetList(typeof(TEntity))).AsQueryable();
            return entityCollection?.Where(cond);
        }

        protected IEnumerable<object> GetList(Type type)
        {
            Type DbContextType = this.GetType();
            foreach (PropertyInfo propertyInfo in DbContextType.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (propertyInfo.PropertyType.GetInterface(typeof(IEnumerable<>).Name) != null && propertyInfo.Name == type.Name)
                    return (IEnumerable<object>)propertyInfo.GetValue(this);
            }
            throw new NotDeclaredEntityException($"{type.Name} are not declare in context as ICollection<{type.Name}>",
                new Exception("You most declare the entity as private ICollection<> property"));
        }

        protected object GetCollection(Type type)
        {
            Type DbContextType = this.GetType();
            foreach (PropertyInfo propertyInfo in DbContextType.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (propertyInfo.PropertyType.Name == typeof(ICollection<>).Name && propertyInfo.Name == type.Name)
                    return propertyInfo.GetValue(this);
            }
            throw new NotDeclaredEntityException($"{type.Name} are not declare in context as ICollection<{type.Name}>",
                new Exception("You most declare the entity as private ICollection<> property"));
        }

        protected void UpdateCollection(Type type, object collection)
        {
            PropertyInfo prop = this.GetType().GetProperty(type.Name, BindingFlags.NonPublic | BindingFlags.Instance);
            prop.SetValue(this, collection);
        }

        public void Update<TEntity>(TEntity entity)
            where TEntity : class
        {
            ICollection<TEntity> collection = (ICollection<TEntity>)GetCollection(typeof(TEntity));
            List<TEntity> nCollection = new List<TEntity>();
            PropertyInfo keyPropertyInfo = AutogenerateId ?
                DynamicLambdaBuilder.GetKeyProperty<TEntity>(typeof(PersistenceIdAttribute)) : 
                entity.GetType().GetProperty("Id")
                ;
                
            foreach (var _entity in collection)
            {
                if (keyPropertyInfo.GetValue(entity).Equals(keyPropertyInfo.GetValue(_entity)))
                        continue;
                nCollection.Add(_entity);
            }
            nCollection.Add(entity);
            UpdateCollection(typeof(TEntity), nCollection);
        }

        public void Update<TEntity>(Expression<Func<TEntity, bool>> cond, TEntity entity) where TEntity : class
        {
            ICollection<TEntity> collection = (ICollection<TEntity>)GetCollection(typeof(TEntity));
            List<TEntity> nCollection = new List<TEntity>();
            foreach (var _entity in collection)
            {
                if (cond.Compile()(_entity))
                    continue;
                
                nCollection.Add(_entity);
            }

            nCollection.Add(entity);
            UpdateCollection(typeof(TEntity), nCollection);
        }

        public void Remove<TEntity>(TEntity entity)
            where TEntity : class
        {
            ICollection<TEntity> collection = (ICollection<TEntity>)GetCollection(typeof(TEntity));
            TEntity entityStored = collection.SingleOrDefault(DynamicLambdaBuilder.GetIdLE(entity).Compile());
            collection.Remove(entityStored);
        }
    }
}
