using System;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;

namespace Isap.Abp.Extensions.Data
{
	public interface IIsapDbContextProviderResolver
	{
		IIsapDbContextProvider GetProvider<TEntity>() where TEntity: class;
	}

	public class IsapDbContextProviderResolver: IIsapDbContextProviderResolver, ITransientDependency
	{
		public IServiceProvider ServiceProvider { get; set; }

		public IIsapDbContextProvider GetProvider<TEntity>() where TEntity: class
		{
			foreach (IIsapDbContextProvider dbContextProvider in ServiceProvider.GetServices<IIsapDbContextProvider>())
			{
				if (dbContextProvider.IsForEntity<TEntity>())
					return dbContextProvider;
			}

			throw new InvalidOperationException($"Can't resolve db context provider for entity type = '{typeof(TEntity)}'.");
		}
	}
}
