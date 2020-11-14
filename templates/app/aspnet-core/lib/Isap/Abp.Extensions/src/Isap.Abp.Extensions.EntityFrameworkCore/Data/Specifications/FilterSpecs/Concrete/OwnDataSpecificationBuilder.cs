using System;
using Isap.CommonCore.Services;

namespace Isap.Abp.Extensions.Data.Specifications.FilterSpecs.Concrete
{
	public class OwnDataSpecificationBuilder<TEntity>: SpecificationBuilderBase<TEntity, object>
		where TEntity: ICommonOwnedEntity<Guid?>
	{
		public override ISpecification<TEntity> Create(ISpecificationBuildingContext context, object parameters)
		{
			return new OwnDataSpecification<TEntity>(context.CurrentUser);
		}
	}
}
