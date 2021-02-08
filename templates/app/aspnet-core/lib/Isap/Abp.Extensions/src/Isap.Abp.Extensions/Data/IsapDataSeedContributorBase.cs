using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace Isap.Abp.Extensions.Data
{
	public abstract class IsapDataSeedContributorBase: IDataSeedContributor
	{
		public IAbpLazyServiceProvider LazyServiceProvider { get; set; }

		public abstract Task SeedAsync(DataSeedContext context);
	}
}
