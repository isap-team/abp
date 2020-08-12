using System.Collections;
using System.Collections.Generic;
using Castle.MicroKernel;

namespace Isap.CommonCore.Factories
{
	public interface IFactory<TProduct>
		where TProduct: class
	{
		ICollection<string> KnownProducts { get; }
		bool IsKnownProduct(string productKey);
		TProduct Create(string productKey);
		TProduct Create(string productKey, Arguments arguments);
		TProduct Create(string productKey, object arguments);
		IDisposable<TProduct> CreateAsDisposable(string productKey);
		IDisposable<TProduct> CreateAsDisposable(string productKey, IDictionary arguments);
		IDisposable<TProduct> CreateAsDisposable(string productKey, object arguments);
		void Release(TProduct product);
	}
}
