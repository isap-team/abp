using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Isap.Abp.Extensions.Expressions
{
	public interface IIncludeExpression
	{
		IIncludeExpressionRegistry IncludeRegistry { get; }
		Func<IQueryable, IQueryable> CreateInclude();
		Func<IQueryable, IQueryable> CreateThenInclude();
	}

	public interface IIncludeExpression<TEntity>: IIncludeExpression
		where TEntity: class, IEntity
	{
		IQueryable ThenInclude(IQueryable query, Func<IQueryable, IQueryable> createInclude);
		Task EnsureLoadedAsync(TEntity entry);
	}

	public interface IIncludeExpression<TEntity, TProperty>: IIncludeExpression<TEntity>
		where TEntity: class, IEntity
	{
		Expression<Func<TEntity, TProperty>> Expression { get; }
	}
}
