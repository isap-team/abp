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
	public class PropertyIncludeExpression<TTopEntity, TEntity, TProperty>: IncludeExpressionBase<TEntity, TProperty>
		where TTopEntity: class
		where TEntity: class, IEntity
		where TProperty: class, IEntity
	{
		public PropertyIncludeExpression([NotNull] IIsapDbContextProvider dbContextProvider, Expression<Func<TEntity, TProperty>> expression)
			: base(expression)
		{
			DbContextProvider = dbContextProvider ?? throw new ArgumentNullException(nameof(dbContextProvider));
			IncludeRegistry = new IncludeExpressionRegistry<TTopEntity, TProperty>(dbContextProvider)
				;
		}

		[NotNull]
		public IIsapDbContextProvider DbContextProvider { get; }

		public IIncludeExpressionRegistry<TProperty> IncludeRegistry { get; }

		private bool IsEntityType()
		{
			if (Expression.Body.NodeType != ExpressionType.MemberAccess)
				return true;

			MemberExpression baseProperty = Expression.Body as MemberExpression;
			Type targetType = baseProperty?.Member.DeclaringType;
			return typeof(IEntity).IsAssignableFrom(targetType);
		}

		public override async Task EnsureLoadedAsync(TEntity entry)
		{
			if (IsEntityType())
			{
				IEfCoreDbContext dbContext = DbContextProvider.GetDbContext<TEntity>();
				EntityEntry<TEntity> entityEntry = dbContext.Entry(entry);
				if (entityEntry.State == EntityState.Detached)
					entityEntry = dbContext.Attach(entry);
				await entityEntry.Reference(Expression).LoadAsync();
			}

			TProperty property = Expression.Compile()(entry);
			if (property != null)
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
			return query =>
				{
					switch (query)
					{
						case IIncludableQueryable<TTopEntity, TEntity> q1:
							return q1.ThenInclude(Expression);
						case IIncludableQueryable<TTopEntity, IEnumerable<TEntity>> q2:
							return q2.ThenInclude(Expression);
						default:
							throw new NotSupportedException();
					}
				};
		}
	}
}
