using System.Linq;
using Isap.Abp.Extensions.Data;
using Volo.Abp.DependencyInjection;

namespace Isap.Abp.Extensions.EntityFrameworkCore
{
	public class DefaultDbSetProvider: IDbSetProvider, ITransientDependency
	{
		public DefaultDbSetProvider(IIsapDbContextProvider dbContextProvider)
		{
			DbContextProvider = dbContextProvider;
		}

		protected IIsapDbContextProvider DbContextProvider { get; }

		public IQueryable<TEntity> GetDbSet<TEntity>()
			where TEntity: class
		{
			return DbContextProvider.Set<TEntity>();
		}
	}
}
