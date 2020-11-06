using System;
using Isap.Abp.Extensions.Data.Specifications;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;

namespace Isap.Abp.Extensions
{
	public class IsapAbpConventionalRegistrar: ConventionalRegistrarBase
	{
		public override void AddType(IServiceCollection services, Type type)
		{
			if (typeof(ISpecificationBuilderRepositoryRegistrationConsumer).IsAssignableFrom(type))
			{
				var consumer = (ISpecificationBuilderRepositoryRegistrationConsumer) Activator.CreateInstance(type);
				var repository = services.GetSingletonInstance<SpecificationBuilderRepository>();
				consumer.RegisterBuilders(repository);
			}
		}
	}
}
