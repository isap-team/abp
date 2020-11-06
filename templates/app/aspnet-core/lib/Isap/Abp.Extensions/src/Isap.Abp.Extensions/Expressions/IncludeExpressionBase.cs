using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Isap.Abp.Extensions.Expressions
{
	public abstract class IncludeExpressionBase<TEntity, TProperty>: IIncludeExpression<TEntity, TProperty>
		where TEntity: class, IEntity
	{
		protected IncludeExpressionBase(Expression<Func<TEntity, TProperty>> expression)
		{
			Expression = expression;
		}

		public Expression<Func<TEntity, TProperty>> Expression { get; }

		IIncludeExpressionRegistry IIncludeExpression.IncludeRegistry => GetIncludeRegistry();

		public abstract Func<IQueryable, IQueryable> CreateInclude();

		public abstract Func<IQueryable, IQueryable> CreateThenInclude();

		public abstract IQueryable ThenInclude(IQueryable query, Func<IQueryable, IQueryable> createInclude);

		public abstract Task EnsureLoadedAsync(TEntity entry);

		protected abstract IIncludeExpressionRegistry GetIncludeRegistry();
	}
}
