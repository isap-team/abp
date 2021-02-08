using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Xunit;

namespace Isap.Abp.FileStorage.Samples
{
	/* Write your custom repository tests like that, in this project, as abstract classes.
	 * Then inherit these abstract classes from EF Core & MongoDB test projects.
	 * In this way, both database providers are tests with the same set tests.
	 */
	public abstract class SampleRepositoryTests<TStartupModule>: FileStorageTestBase<TStartupModule>
		where TStartupModule: IAbpModule
	{
		//private readonly ISampleRepository _sampleRepository;

		protected SampleRepositoryTests()
		{
			//_sampleRepository = GetRequiredService<ISampleRepository>();
		}

		[Fact]
		public Task Method1Async()
		{
			return Task.CompletedTask;
		}
	}
}
