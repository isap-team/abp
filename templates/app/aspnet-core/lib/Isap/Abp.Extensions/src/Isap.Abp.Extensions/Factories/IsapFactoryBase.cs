using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Castle.MicroKernel;
using Isap.CommonCore;
using Isap.CommonCore.Factories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.DependencyInjection;

namespace Isap.Abp.Extensions.Factories
{
	public interface IIsapFactoryBuilder
	{
		void RegisterProducts(Assembly assembly, IServiceCollection services);
	}

	public abstract class IsapFactoryBase<TProduct>: IFactory<TProduct>, IIsapFactoryBuilder
		where TProduct: class
	{
		private class ProductWrapper: DisposableBase<TProduct>
		{
			private readonly IFactory<TProduct> _factory;
			private readonly TProduct _product;

			public ProductWrapper(IFactory<TProduct> factory, TProduct product)
			{
				_factory = factory;
				_product = product;
			}

			protected override TProduct GetWrappedObject()
			{
				return _product;
			}

			protected override void InternalDispose()
			{
				_factory.Release(_product);
			}
		}

		private readonly Dictionary<string, Type> _productMap = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

		protected IsapFactoryBase(
			ObjectAccessor<IServiceProvider> serviceProviderAccessor)
		{
			ServiceProviderAccessor = serviceProviderAccessor;
		}

		public ObjectAccessor<IServiceProvider> ServiceProviderAccessor { get; }

		public ICollection<string> KnownProducts => _productMap.Keys;

		public bool IsKnownProduct(string productKey) => _productMap.ContainsKey(productKey);

		public TProduct Create(string productKey)
		{
			return (TProduct) ServiceProviderAccessor.Value.GetRequiredService(_productMap[productKey]);
		}

		TProduct IFactory<TProduct>.Create(string productKey, Arguments arguments) => throw new NotSupportedException();

		TProduct IFactory<TProduct>.Create(string productKey, object arguments) => throw new NotSupportedException();

		public IDisposable<TProduct> CreateAsDisposable(string productKey)
		{
			return new ProductWrapper(this, Create(productKey));
		}

		public IDisposable<TProduct> CreateAsDisposable(string productKey, IDictionary arguments) => throw new NotSupportedException();

		public IDisposable<TProduct> CreateAsDisposable(string productKey, object arguments) => throw new NotSupportedException();

		public void Release(TProduct product)
		{
		}

		public void RegisterProducts(Assembly assembly, IServiceCollection services)
		{
			void RegisterProduct(string productKey, Type productType)
			{
				_productMap[productKey] = productType;
				services.TryAddTransient(productType);
			}

			RegisterProducts(assembly, RegisterProduct);
		}

		protected abstract void RegisterProducts(Assembly assembly, Action<string, Type> registerProduct);
	}
}
