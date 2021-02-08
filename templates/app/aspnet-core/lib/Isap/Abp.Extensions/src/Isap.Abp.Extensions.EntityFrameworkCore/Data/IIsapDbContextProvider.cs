using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Isap.Abp.Extensions.Data
{
	public interface IIsapDbContextProvider
	{
		bool IsForEntity<TEntity>() where TEntity: class;
		IEfCoreDbContext GetDbContext<TEntity>() where TEntity: class;
		DbSet<TEntity> Set<TEntity>() where TEntity: class;
	}
}
