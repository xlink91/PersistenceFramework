using System;
using System.Linq.Expressions;

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
    }
}
