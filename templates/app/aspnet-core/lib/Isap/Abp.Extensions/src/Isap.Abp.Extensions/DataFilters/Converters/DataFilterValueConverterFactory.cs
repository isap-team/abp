using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel;
using Isap.CommonCore;
using Isap.CommonCore.Factories;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;

namespace Isap.Abp.Extensions.DataFilters.Converters
{
	[AttributeUsage(AttributeTargets.Class)]
	public class DataFilterValueConverterAttribute: Attribute, IFactoryProductMarker
	{
		public DataFilterValueConverterAttribute(string name)
		{
			Name = name;
		}

		public string Name { get; }
		string IFactoryProductMarker.ProductKey => Name;
	}

	public interface IDataFilterValueConverterFactory: IFactory<IDataFilterValueConverter>
	{
	}

	public interface IDataFilterValueConverterFactoryBuilder
	{
		void RegisterProducts(Assembly assembly, IServiceCollection services);
	}

	public class DataFilterValueConverterFactory: IDataFilterValueConverterFactory, IDataFilterValueConverterFactoryBuilder
	{
		private class ProductWrapper: DisposableBase<IDataFilterValueConverter>
		{
			private readonly IFactory<IDataFilterValueConverter> _factory;
			private readonly IDataFilterValueConverter _product;

			public ProductWrapper(IFactory<IDataFilterValueConverter> factory, IDataFilterValueConverter product)
			{
				_factory = factory;
				_product = product;
			}

			protected override IDataFilterValueConverter GetWrappedObject()
			{
				return _product;
			}

			protected override void InternalDispose()
			{
				_factory.Release(_product);
			}
		}

		private readonly Dictionary<string, Type> _productMap = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

		public DataFilterValueConverterFactory(
			ObjectAccessor<IServiceProvider> serviceProviderAccessor)
		{
			ServiceProviderAccessor = serviceProviderAccessor;
		}

		public ObjectAccessor<IServiceProvider> ServiceProviderAccessor { get; }

		public ICollection<string> KnownProducts => _productMap.Keys;

		public bool IsKnownProduct(string productKey)
		{
			return _productMap.ContainsKey(productKey);
		}

		public IDataFilterValueConverter Create(string productKey)
		{
			return (IDataFilterValueConverter) ServiceProviderAccessor.Value.GetRequiredService(_productMap[productKey]);
		}

		IDataFilterValueConverter IFactory<IDataFilterValueConverter>.Create(string productKey, Arguments arguments)
		{
			throw new NotSupportedException();
		}

		IDataFilterValueConverter IFactory<IDataFilterValueConverter>.Create(string productKey, object arguments)
		{
			throw new NotSupportedException();
		}

		IDisposable<IDataFilterValueConverter> IFactory<IDataFilterValueConverter>.CreateAsDisposable(string productKey)
		{
			return new ProductWrapper(this, Create(productKey));
		}

		IDisposable<IDataFilterValueConverter> IFactory<IDataFilterValueConverter>.CreateAsDisposable(string productKey, IDictionary arguments)
		{
			throw new NotSupportedException();
		}

		IDisposable<IDataFilterValueConverter> IFactory<IDataFilterValueConverter>.CreateAsDisposable(string productKey, object arguments)
		{
			throw new NotSupportedException();
		}

		void IFactory<IDataFilterValueConverter>.Release(IDataFilterValueConverter product)
		{
		}

		public void RegisterProducts(Assembly assembly, IServiceCollection services)
		{
			List<Tuple<Type, IFactoryProductMarker>> tuples = assembly.GetTypes()
				.Where(t => typeof(IDataFilterValueConverter).IsAssignableFrom(t))
				.Select(t => Tuple.Create(t, (IFactoryProductMarker) t.GetCustomAttribute<DataFilterValueConverterAttribute>()))
				.Where(tuple => tuple.Item2 != null)
				.ToList();
			foreach (Tuple<Type, IFactoryProductMarker> tuple in tuples)
			{
				_productMap[tuple.Item2.ProductKey] = tuple.Item1;
				services.AddTransient(tuple.Item1);
			}
		}
	}
}
