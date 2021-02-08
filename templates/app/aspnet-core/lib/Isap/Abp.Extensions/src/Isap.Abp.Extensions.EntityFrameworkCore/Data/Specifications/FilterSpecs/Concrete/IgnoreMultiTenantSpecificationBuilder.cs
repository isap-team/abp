namespace Isap.Abp.Extensions.Data.Specifications.FilterSpecs.Concrete
{
	public class IgnoreMultiTenantSpecificationBuilder<TEntity>: SpecificationBuilderBase<TEntity, object>
	{
		public override ISpecification<TEntity> Create(ISpecificationBuildingContext context, object parameters)
		{
			return new IgnoreMultiTenantSpecification<TEntity>();
		}
	}
}
