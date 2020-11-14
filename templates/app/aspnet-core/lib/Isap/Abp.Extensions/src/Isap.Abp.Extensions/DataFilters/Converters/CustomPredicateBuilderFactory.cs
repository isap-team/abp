using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Isap.Abp.Extensions.Factories;
using Isap.CommonCore.Factories;
using Volo.Abp.DependencyInjection;

namespace Isap.Abp.Extensions.DataFilters.Converters
{
	[AttributeUsage(AttributeTargets.Class)]
	public class CustomPredicateBuilderAttribute: Attribute, IFactoryProductMarker
	{
		public CustomPredicateBuilderAttribute(string name)
		{
			Name = name;
		}

		public string Name { get; }
		string IFactoryProductMarker.ProductKey => Name;
	}

	public interface ICustomPredicateBuilderFactory: IFactory<ICustomPredicateBuilder>
	{
	}

	public interface ICustomPredicateBuilderFactoryBuilder: IIsapFactoryBuilder
	{
	}

	public class CustomPredicateBuilderFactory: IsapFactoryBase<ICustomPredicateBuilder>, ICustomPredicateBuilderFactory, ICustomPredicateBuilderFactoryBuilder
	{
		public CustomPredicateBuilderFactory(ObjectAccessor<IServiceProvider> serviceProviderAccessor)
			: base(serviceProviderAccessor)
		{
		}

		protected override void RegisterProducts(Assembly assembly, Action<string, Type> registerProduct)
		{
			List<Tuple<Type, IFactoryProductMarker>> tuples = assembly.GetTypes()
				.Where(t => typeof(ICustomPredicateBuilder).IsAssignableFrom(t))
				.Select(t => Tuple.Create(t, (IFactoryProductMarker) t.GetCustomAttribute<CustomPredicateBuilderAttribute>()))
				.Where(tuple => tuple.Item2 != null)
				.ToList();
			foreach (Tuple<Type, IFactoryProductMarker> tuple in tuples)
			{
				registerProduct(tuple.Item2.ProductKey, tuple.Item1);
			}
		}
	}
}
