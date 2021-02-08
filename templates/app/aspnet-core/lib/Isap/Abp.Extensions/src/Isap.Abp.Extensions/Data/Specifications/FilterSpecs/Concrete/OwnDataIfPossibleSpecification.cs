using System;
using System.Linq.Expressions;
using Isap.CommonCore.Services;
using Volo.Abp.Users;

namespace Isap.Abp.Extensions.Data.Specifications.FilterSpecs.Concrete
{
	public class OwnDataIfPossibleSpecification<TEntity>: FilterSpecificationBase<TEntity>
	{
		private readonly IFilterSpecification<TEntity> _specification;

		public OwnDataIfPossibleSpecification(ICurrentUser currentUser)
		{
			if (typeof(ICommonOwnedEntity<Guid?>).IsAssignableFrom(typeof(TEntity)))
			{
				Type type = typeof(OwnDataSpecification<>).MakeGenericType(typeof(TEntity));
				_specification = (IFilterSpecification<TEntity>) Activator.CreateInstance(type, currentUser);
			}
			else
			{
				_specification = new TrueSpecification<TEntity>();
			}
		}

		public override Expression<Func<TEntity, bool>> IsSatisfiedBy()
		{
			return _specification.IsSatisfiedBy();
		}
	}
}
