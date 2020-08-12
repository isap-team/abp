using System.Reflection;

namespace Isap.CommonCore.Factories
{
	public interface IFactoryBuilder<TProduct>
		where TProduct: class
	{
		IFactory<TProduct> Factory { get; }
		void RegisterProducts(Assembly assembly);
	}
}
