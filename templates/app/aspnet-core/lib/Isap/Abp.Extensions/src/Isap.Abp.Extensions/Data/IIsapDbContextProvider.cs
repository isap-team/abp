using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Isap.Abp.Extensions.Data
{
	public interface IIsapDbContextProvider
	{
		IEfCoreDbContext GetDbContext<TEntity>() where TEntity: class;
		DbSet<TEntity> Set<TEntity>() where TEntity: class;
	}
}
