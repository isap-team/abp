using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;

namespace Isap.Abp.Extensions.Data
{
	public interface IIsapDbContextProviderResolver
	{
		Task<IIsapDbContextProvider> GetProvider<TEntity>() where TEntity: class;
	}

	public class IsapDbContextProviderResolver: IIsapDbContextProviderResolver, ITransientDependency
	{
		public IServiceProvider ServiceProvider { get; set; }

		public async Task<IIsapDbContextProvider> GetProvider<TEntity>() where TEntity: class
		{
			foreach (IIsapDbContextProvider dbContextProvider in ServiceProvider.GetServices<IIsapDbContextProvider>())
			{
				if (await dbContextProvider.IsForEntity<TEntity>())
					return dbContextProvider;
			}

			throw new InvalidOperationException($"Can't resolve db context provider for entity type = '{typeof(TEntity)}'.");
		}
	}
}
