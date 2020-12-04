using System;
using Isap.Abp.Extensions.Domain;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Isap.Abp.Extensions.Data
{
	public class IsapUnitOfWorkDbContextProvider<TDbContext>: DomainServiceBase, IIsapDbContextProvider
		where TDbContext: IEfCoreDbContext
	{
		public IsapUnitOfWorkDbContextProvider(IServiceProvider serviceProvider)
		{
			// Обнаружено странное поведение Autofac, когда именно для этого класса у экземпляра не заполняется свойство ServiceProvider,
			// которое должно заполняться через механизм property injection.
			ServiceProvider = serviceProvider;
		}

		protected IDbContextProvider<TDbContext> DbContextProvider => LazyGetRequiredService<IDbContextProvider<TDbContext>>();

		public IEfCoreDbContext GetDbContext<TEntity>() where TEntity: class
		{
			return DbContextProvider.GetDbContext();
		}

		public DbSet<TEntity> Set<TEntity>() where TEntity: class
		{
			return GetDbContext<TEntity>().Set<TEntity>();
		}
	}
}
