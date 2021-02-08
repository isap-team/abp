using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Isap.Abp.Extensions.Querying
{
	public interface ISortExpressionProvider<TEntity>
	{
		IOrderedEnumerable<TEntity> OrderBy(IEnumerable<TEntity> query, bool isDescending);
		IOrderedEnumerable<TEntity> ThenBy(IOrderedEnumerable<TEntity> query, bool isDescending);

		IOrderedQueryable<TEntity> OrderBy(IQueryable<TEntity> query, bool isDescending);
		IOrderedQueryable<TEntity> ThenBy(IOrderedQueryable<TEntity> query, bool isDescending);
	}

	public class SortExpressionProvider<TEntity, TSortKey>: ISortExpressionProvider<TEntity>
	{
		private readonly MemberExpression _memberExpression;
		private readonly ParameterExpression _orderByParameter;

		public SortExpressionProvider(MemberExpression memberExpression, ParameterExpression orderByParameter)
		{
			_memberExpression = memberExpression;
			_orderByParameter = orderByParameter;
		}

		public IOrderedEnumerable<TEntity> OrderBy(IEnumerable<TEntity> query, bool isDescending)
		{
			Expression<Func<TEntity, TSortKey>> expression = Expression.Lambda<Func<TEntity, TSortKey>>(_memberExpression, _orderByParameter);
			return isDescending ? query.OrderByDescending(expression.Compile()) : query.OrderBy(expression.Compile());
		}

		public IOrderedEnumerable<TEntity> ThenBy(IOrderedEnumerable<TEntity> query, bool isDescending)
		{
			Expression<Func<TEntity, TSortKey>> expression = Expression.Lambda<Func<TEntity, TSortKey>>(_memberExpression, _orderByParameter);
			return isDescending ? query.ThenByDescending(expression.Compile()) : query.ThenBy(expression.Compile());
		}

		public IOrderedQueryable<TEntity> OrderBy(IQueryable<TEntity> query, bool isDescending)
		{
			Expression<Func<TEntity, TSortKey>> expression = Expression.Lambda<Func<TEntity, TSortKey>>(_memberExpression, _orderByParameter);
			return isDescending ? query.OrderByDescending(expression) : query.OrderBy(expression);
		}

		public IOrderedQueryable<TEntity> ThenBy(IOrderedQueryable<TEntity> query, bool isDescending)
		{
			Expression<Func<TEntity, TSortKey>> expression = Expression.Lambda<Func<TEntity, TSortKey>>(_memberExpression, _orderByParameter);
			return isDescending ? query.ThenByDescending(expression) : query.ThenBy(expression);
		}
	}
}
