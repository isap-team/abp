using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Data;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Volo.Abp.Domain.Entities;
using Volo.Abp.EntityFrameworkCore;

namespace Isap.Abp.Extensions.Expressions
{
	public class CollectionIncludeExpression<TTopEntity, TEntity, TProperty>: IncludeExpressionBase<TEntity, IEnumerable<TProperty>>
		where TTopEntity: class
		where TEntity: class, IEntity
		where TProperty: class, IEntity
	{
		public CollectionIncludeExpression([NotNull] IIsapDbContextProvider dbContextProvider, Expression<Func<TEntity, IEnumerable<TProperty>>> expression)
			: base(expression)
		{
			DbContextProvider = dbContextProvider ?? throw new ArgumentNullException(nameof(dbContextProvider));
			IncludeRegistry = new IncludeExpressionRegistry<TTopEntity, TProperty>(dbContextProvider);
		}

		[NotNull]
		public IIsapDbContextProvider DbContextProvider { get; }

		public IIncludeExpressionRegistry<TProperty> IncludeRegistry { get; }

		public override async Task EnsureLoadedAsync(TEntity entry)
		{
			Func<TEntity, IEnumerable<TProperty>> getItems = Expression.Compile();
			IEnumerable<TProperty> properties = getItems(entry);
			if (properties == null)
			{
				IEfCoreDbContext dbContext = await DbContextProvider.GetDbContext<TEntity>();
				EntityEntry<TEntity> entityEntry = dbContext.Entry(entry);
				if (entityEntry.State == EntityState.Detached)
					entityEntry = dbContext.Attach(entry);
				await entityEntry.Collection(Expression).LoadAsync();
				properties = getItems(entry);
			}

			if (properties != null)
				foreach (TProperty property in properties)
					await IncludeRegistry.EnsureLoadedAsync(property);
		}

		protected override IIncludeExpressionRegistry GetIncludeRegistry()
		{
			return IncludeRegistry;
		}

		public override IQueryable ThenInclude(IQueryable query, Func<IQueryable, IQueryable> createInclude)
		{
			foreach (IIncludeExpression<TProperty> thenInclude in IncludeRegistry.Includes)
			{
				IQueryable Include(IQueryable q) => thenInclude.CreateThenInclude()(createInclude(q));
				query = Include(query);
				query = thenInclude.ThenInclude(query, Include);
			}

			return query;
		}

		public override Func<IQueryable, IQueryable> CreateInclude()
		{
			return query => ((IQueryable<TEntity>) query).Include(Expression);
		}

		public override Func<IQueryable, IQueryable> CreateThenInclude()
		{
			return query => ((IIncludableQueryable<TTopEntity, TEntity>) query).ThenInclude(Expression);
		}
	}
}
