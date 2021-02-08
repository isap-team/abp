using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Isap.Abp.Extensions.Expressions
{
	public class NullExpressionRegistry<TEntity>: IIncludeExpressionRegistry<TEntity>
		where TEntity: class, IEntity
	{
		public ICollection<IIncludeExpression<TEntity>> Includes { get; } =
			new ReadOnlyCollection<IIncludeExpression<TEntity>>(new List<IIncludeExpression<TEntity>>());

		public IQueryable<TEntity> Include(IQueryable<TEntity> query)
		{
			return query;
		}

		public void RegisterPropertyInclude<TProperty>(Expression<Func<TEntity, TProperty>> expression,
			Action<IIncludeExpressionRegistry<TProperty>> registerChildIncludes) where TProperty: class, IEntity
		{
			throw new InvalidOperationException("Incorrect includes registration.");
		}

		public void RegisterCollectionInclude<TProperty>(Expression<Func<TEntity, IEnumerable<TProperty>>> expression,
			Action<IIncludeExpressionRegistry<TProperty>> registerChildIncludes) where TProperty: class, IEntity
		{
			throw new InvalidOperationException("Incorrect includes registration.");
		}

		public Task EnsureLoadedAsync(TEntity entry)
		{
			return Task.CompletedTask;
		}

		public void RegisterPropertyInclude<TProperty>(Expression<Func<TEntity, TProperty>> expression) where TProperty: class, IEntity
		{
			throw new InvalidOperationException("Incorrect includes registration.");
		}

		public void RegisterCollectionInclude<TProperty>(Expression<Func<TEntity, IEnumerable<TProperty>>> expression) where TProperty: class, IEntity
		{
			throw new InvalidOperationException("Incorrect includes registration.");
		}
	}
}
