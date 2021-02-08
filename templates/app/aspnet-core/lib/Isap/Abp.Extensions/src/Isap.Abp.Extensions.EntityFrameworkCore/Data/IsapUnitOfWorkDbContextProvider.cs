using System;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
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

		protected IDbContextProvider<TDbContext> DbContextProvider => LazyServiceProvider.LazyGetRequiredService<IDbContextProvider<TDbContext>>();

		public async Task<bool> IsForEntity<TEntity>() where TEntity: class
		{
#pragma warning disable EF1001 // Internal EF Core API usage.
			IEntityType entityType = (await DbContextProvider.GetDbContextAsync()).Model.FindEntityType(typeof(TEntity));
#pragma warning restore EF1001 // Internal EF Core API usage.
			return entityType != null;
		}

		public async Task<IEfCoreDbContext> GetDbContext<TEntity>() where TEntity: class
		{
			return await DbContextProvider.GetDbContextAsync();
		}

		public async Task<DbSet<TEntity>> Set<TEntity>() where TEntity: class
		{
			return (await GetDbContext<TEntity>()).Set<TEntity>();
		}
	}
}
