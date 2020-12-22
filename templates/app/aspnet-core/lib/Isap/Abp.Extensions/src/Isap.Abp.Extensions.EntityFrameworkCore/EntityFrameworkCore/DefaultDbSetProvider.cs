using System.Linq;
using Isap.Abp.Extensions.Data;
using Volo.Abp.DependencyInjection;

namespace Isap.Abp.Extensions.EntityFrameworkCore
{
	public class DefaultDbSetProvider: IDbSetProvider, ITransientDependency
	{
		public DefaultDbSetProvider(IIsapDbContextProviderResolver dbContextProviderResolver)
		{
			DbContextProviderResolver = dbContextProviderResolver;
		}

		protected IIsapDbContextProviderResolver DbContextProviderResolver { get; }

		public IQueryable<TEntity> GetDbSet<TEntity>()
			where TEntity: class
		{
			return DbContextProviderResolver.GetProvider<TEntity>().Set<TEntity>();
		}
	}
}
