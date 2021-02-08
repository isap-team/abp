using System;
using System.Linq.Expressions;
using Isap.CommonCore.Services;
using Volo.Abp.Users;

namespace Isap.Abp.Extensions.Data.Specifications.FilterSpecs.Concrete
{
	public class OwnDataSpecification<TEntity>: FilterSpecificationBase<TEntity>
		where TEntity: ICommonOwnedEntity<Guid?>
	{
		private readonly ICurrentUser _currentUser;

		public OwnDataSpecification(ICurrentUser currentUser)
		{
			_currentUser = currentUser;
		}

		public override Expression<Func<TEntity, bool>> IsSatisfiedBy()
		{
			Guid currentUserId = _currentUser.GetId();
			return e => e.OwnerId == currentUserId;
		}
	}
}
