using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Isap.Abp.Extensions.Expressions
{
	public interface IIncludeExpressionRegistry
	{
	}

	public interface IIncludeExpressionRegistry<TEntity>: IIncludeExpressionRegistry
		where TEntity: class, IEntity
	{
		/// <summary>
		///     Список оберток для
		///     <see cref="Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.Include{TEntity,TProperty}" />
		///     выражений.
		/// </summary>
		ICollection<IIncludeExpression<TEntity>> Includes { get; }

		IQueryable<TEntity> Include(IQueryable<TEntity> query);

		void RegisterPropertyInclude<TProperty>(Expression<Func<TEntity, TProperty>> expression,
			Action<IIncludeExpressionRegistry<TProperty>> registerChildIncludes = null)
			where TProperty: class, IEntity;

		void RegisterCollectionInclude<TProperty>(Expression<Func<TEntity, IEnumerable<TProperty>>> expression,
			Action<IIncludeExpressionRegistry<TProperty>> registerChildIncludes = null)
			where TProperty: class, IEntity;

		Task EnsureLoadedAsync(TEntity entry);
	}
}
