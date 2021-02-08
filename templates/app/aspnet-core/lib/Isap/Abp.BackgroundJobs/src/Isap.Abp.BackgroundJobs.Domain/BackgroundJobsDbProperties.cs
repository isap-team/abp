using Volo.Abp.Data;

namespace Isap.Abp.BackgroundJobs
{
	public static class BackgroundJobsDbProperties
	{
		public const string ConnectionStringName = "AbpBackgroundJobs";

		public static string DbTablePrefix { get; set; } = AbpCommonDbProperties.DbTablePrefix;

		public static string DbSchema { get; set; } = AbpCommonDbProperties.DbSchema;
	}
}
