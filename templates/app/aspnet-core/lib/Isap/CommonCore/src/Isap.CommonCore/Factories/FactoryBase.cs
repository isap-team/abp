using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Castle.Core.Internal;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Isap.CommonCore.Extensions;

namespace Isap.CommonCore.Factories
{
	public abstract class FactoryBase<TProduct, TMarkerAttr>: IFactory<TProduct>, IFactoryBuilder<TProduct>
		where TProduct: class
		where TMarkerAttr: Attribute, IFactoryProductMarker
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

		private readonly IList<string> _knownProducts = new List<string>();

		protected FactoryBase(IKernel container)
		{
			Container = container;
		}

		protected IKernel Container { get; }

		IFactory<TProduct> IFactoryBuilder<TProduct>.Factory => this;

		public ICollection<string> KnownProducts => new ReadOnlyCollection<string>(_knownProducts);

		public bool IsKnownProduct(string productKey)
		{
			return Container.HasComponent(typeof(TProduct)) && Container.HasComponent(GetProductFullKey(productKey));
		}

		public virtual TProduct Create(string productKey)
		{
			return Container.Resolve<TProduct>(GetProductFullKey(productKey));
		}

		public virtual TProduct Create(string productKey, Arguments arguments)
		{
			return Container.Resolve<TProduct>(GetProductFullKey(productKey), arguments);
		}

		public TProduct Create(string productKey, object arguments)
		{
			var args = new Arguments
				{
					arguments.AsNameToObjectMap().Select(pair => new KeyValuePair<object, object>(pair.Key, pair.Value))
				};
			return Container.Resolve<TProduct>(GetProductFullKey(productKey), args);
		}

		public IDisposable<TProduct> CreateAsDisposable(string productKey)
		{
			return new ProductWrapper(this, Create(productKey));
		}

		public IDisposable<TProduct> CreateAsDisposable(string productKey, IDictionary arguments)
		{
			return new ProductWrapper(this, Create(productKey, arguments));
		}

		public IDisposable<TProduct> CreateAsDisposable(string productKey, object arguments)
		{
			return new ProductWrapper(this, Create(productKey, arguments));
		}

		public void Release(TProduct product)
		{
			Container.ReleasePolicy.Release(product);
		}

		public void RegisterProducts(Assembly assembly)
		{
			List<Tuple<string, Type>> types = assembly.GetTypes()
				.Where(type => typeof(TProduct).IsAssignableFrom(type))
				.SelectMany(type => type.GetAttributes<TMarkerAttr>()
					.Select(attr => attr.ProductKey)
					.Distinct(StringComparer.OrdinalIgnoreCase)
					.Select(productKey => Tuple.Create(productKey, type)))
				.ToList();
			foreach (Tuple<string, Type> tuple in types)
			{
				_knownProducts.Add(tuple.Item1);
				Container.Register(GetComponentRegistration(tuple.Item1, tuple.Item2));
			}
		}

		protected virtual ComponentRegistration<TProduct> GetComponentRegistration(string productKey, Type productType)
		{
			return Component.For<TProduct>().ImplementedBy(productType).Named(GetProductFullKey(productKey));
		}

		private static string GetProductFullKey(string productKey)
		{
			return $"{typeof(TProduct).FullName}:{productKey}";
		}
	}
}
