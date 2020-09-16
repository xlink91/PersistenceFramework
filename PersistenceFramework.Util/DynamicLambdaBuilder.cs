using PersistenceFramework.Exceptions;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace PersistenceFramework.Util
{
    public static class DynamicLambdaBuilder
    {
        public static Expression<Func<TEntity, bool>> GetIdLE<TEntity>(TEntity entity)
        {
            ParameterExpression param = Expression.Parameter(typeof(TEntity), "x");
            MemberExpression memberExpression = Expression.PropertyOrField(param, "Id");

            BinaryExpression equal = Expression.Equal(memberExpression, Expression.Constant(entity.GetType().GetProperty("Id").GetValue(entity)));

            return Expression.Lambda<Func<TEntity, bool>>(equal, param);
        }

        public static Expression<Func<TEntity, bool>> GetIdLE<TEntity>(TEntity entity, Type attribute)
        {
            var propertyInfo = GetKeyProperty(entity, attribute);

            if (propertyInfo == null)
                return GetIdLE(entity);

            ParameterExpression param = Expression.Parameter(typeof(TEntity), "x");
            MemberExpression memberExpression = Expression.PropertyOrField(param, propertyInfo.Name);

            BinaryExpression equal = Expression.Equal(memberExpression, Expression.Constant(propertyInfo.GetValue(entity)));

            return Expression.Lambda<Func<TEntity, bool>>(equal, param);
        }

        public static Expression<Func<TEntity, bool>> GetComplementIdLE<TEntity>(TEntity entity, Type attribute)
        {
            var propertyInfo = GetKeyProperty(entity, attribute);

            if (propertyInfo == null)
                return GetIdLE(entity);

            ParameterExpression param = Expression.Parameter(typeof(TEntity), "x");
            MemberExpression memberExpression = Expression.PropertyOrField(param, propertyInfo.Name);

            BinaryExpression equal = Expression.NotEqual(memberExpression, Expression.Constant(propertyInfo.GetValue(entity)));

            return Expression.Lambda<Func<TEntity, bool>>(equal, param);
        }

        public static PropertyInfo GetKeyProperty<TEntity>(TEntity entity, Type attribute)
        {
            var attributeIdCounter = 0;
            PropertyInfo idProperty = default;

            foreach (var propertyInfo in entity.GetType().GetProperties())
            {
                if (propertyInfo.GetCustomAttribute(attribute) != null)
                {
                    idProperty = propertyInfo;
                    ++attributeIdCounter;
                }
            }

            if (attributeIdCounter > 1)
                throw new DuplicateIdException();

            return idProperty;
        }

        public static PropertyInfo GetKeyProperty<TEntity>(Type attribute)
        {
            var attributeIdCounter = 0;
            Type entityType = typeof(TEntity);
            PropertyInfo idProperty = default;

            foreach (var propertyInfo in entityType.GetProperties())
            {
                if (propertyInfo.GetCustomAttribute(attribute) != null)
                {
                    idProperty = propertyInfo;
                    ++attributeIdCounter;
                }
            }

            if (attributeIdCounter > 1)
                throw new DuplicateIdException();

            return idProperty;
        }

        public static Expression<Func<TEntity, bool>> CreateFilterForCollection<TEntity, TResult>(
                                                        Expression<Func<TEntity, TResult>> filter, TEntity value)
        {
            var unaryExpression = filter.Body as MemberExpression;
            var leftExpression = unaryExpression;
            var rightExpression = Expression.Constant(filter.Compile()(value));
            var body = Expression.MakeBinary(ExpressionType.Equal, leftExpression, rightExpression);

            Expression<Func<TEntity, bool>> final_expression = 
                Expression.Lambda<Func<TEntity, bool>>(body, filter.Parameters[0]);

            return final_expression;
        }
    }
}
