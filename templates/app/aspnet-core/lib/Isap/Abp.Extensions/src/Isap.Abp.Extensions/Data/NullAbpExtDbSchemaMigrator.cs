using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Isap.Abp.Extensions.Data
{
	/* This is used if database provider doesn't define
	 * IIdentityServerDbSchemaMigrator implementation.
	 */
	public class NullAbpExtDbSchemaMigrator: IAbpExtDbSchemaMigrator, ITransientDependency
	{
		public Task MigrateAsync()
		{
			return Task.CompletedTask;
		}
	}
}
