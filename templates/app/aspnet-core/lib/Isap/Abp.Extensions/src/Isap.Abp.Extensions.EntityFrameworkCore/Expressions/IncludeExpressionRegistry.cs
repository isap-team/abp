using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Data;
using JetBrains.Annotations;
using Volo.Abp.Domain.Entities;

namespace Isap.Abp.Extensions.Expressions
{
	public class IncludeExpressionRegistry<TTopEntity, TEntity>: IIncludeExpressionRegistry<TEntity>
		where TTopEntity: class
		where TEntity: class, IEntity
	{
		public IncludeExpressionRegistry([NotNull] IIsapDbContextProvider dbContextProvider)
		{
			DbContextProvider = dbContextProvider ?? throw new ArgumentNullException(nameof(dbContextProvider));
			Includes = new List<IIncludeExpression<TEntity>>();
		}

		[NotNull]
		public IIsapDbContextProvider DbContextProvider { get; }

		public ICollection<IIncludeExpression<TEntity>> Includes { get; }

		public IQueryable<TEntity> Include(IQueryable<TEntity> query)
		{
			IQueryable q = query;
			foreach (IIncludeExpression<TEntity> include in Includes)
			{
				Func<IQueryable, IQueryable> createInclude = include.CreateInclude();
				q = createInclude(q);
				q = include.ThenInclude(q, createInclude);
			}

			return (IQueryable<TEntity>) q;
		}

		public virtual void RegisterPropertyInclude<TProperty>(Expression<Func<TEntity, TProperty>> expression,
			Action<IIncludeExpressionRegistry<TProperty>> registerChildIncludes = null)
			where TProperty: class, IEntity
		{
			PropertyIncludeExpression<TTopEntity, TEntity, TProperty> include =
				new PropertyIncludeExpression<TTopEntity, TEntity, TProperty>(DbContextProvider, expression);
			Includes.Add(include);
			registerChildIncludes?.Invoke(include.IncludeRegistry);
		}

		public virtual void RegisterCollectionInclude<TProperty>(Expression<Func<TEntity, IEnumerable<TProperty>>> expression,
			Action<IIncludeExpressionRegistry<TProperty>> registerChildIncludes = null)
			where TProperty: class, IEntity
		{
			CollectionIncludeExpression<TTopEntity, TEntity, TProperty> include =
				new CollectionIncludeExpression<TTopEntity, TEntity, TProperty>(DbContextProvider, expression);
			Includes.Add(include);
			registerChildIncludes?.Invoke(include.IncludeRegistry);
		}

		public async Task EnsureLoadedAsync(TEntity entry)
		{
			foreach (IIncludeExpression<TEntity> include in Includes)
				await include.EnsureLoadedAsync(entry);
		}
	}
}
