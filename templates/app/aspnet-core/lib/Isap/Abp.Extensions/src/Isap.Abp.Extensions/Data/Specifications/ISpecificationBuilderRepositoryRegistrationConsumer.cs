namespace Isap.Abp.Extensions.Data.Specifications
{
	public interface ISpecificationBuilderRepositoryRegistrationConsumer
	{
		void RegisterBuilders(ISpecificationBuilderRepositoryRegistrar repository);
	}
}
