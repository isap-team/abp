using System;
using System.Linq;
using System.Reflection;
using Isap.Abp.Extensions.Data.Specifications;
using Isap.Abp.Extensions.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Isap.CommonCore.Metadata;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;

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
			else if (typeof(IEntity).IsAssignableFrom(type) || type.GetCustomAttributes<EntityDefAttribute>().Any())
			{
				var metadataRegistrar = services.GetSingletonInstance<MetadataProvider>();
				IEntityDefinition entityDef = EntityDefinition.CreateBuilder(type).Build();
				metadataRegistrar.Register(entityDef, false);
			}
		}
	}
}
