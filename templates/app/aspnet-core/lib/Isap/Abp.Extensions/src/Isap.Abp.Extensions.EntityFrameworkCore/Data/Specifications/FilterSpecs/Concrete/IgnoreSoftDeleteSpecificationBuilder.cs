namespace Isap.Abp.Extensions.Data.Specifications.FilterSpecs.Concrete
{
	public class IgnoreSoftDeleteSpecificationBuilder<TEntity>: SpecificationBuilderBase<TEntity, object>
	{
		public override ISpecification<TEntity> Create(ISpecificationBuildingContext context, object parameters)
		{
			return new IgnoreSoftDeleteSpecification<TEntity>();
		}
	}
}