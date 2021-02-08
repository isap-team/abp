using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Isap.Abp.Extensions.Data
{
	public interface IIsapDbContextProvider
	{
		Task<bool> IsForEntity<TEntity>() where TEntity: class;
		Task<IEfCoreDbContext> GetDbContext<TEntity>() where TEntity: class;
		Task<DbSet<TEntity>> Set<TEntity>() where TEntity: class;
	}
}
