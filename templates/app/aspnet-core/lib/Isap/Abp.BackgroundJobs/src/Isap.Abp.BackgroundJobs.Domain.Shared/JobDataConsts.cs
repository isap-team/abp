namespace Isap.Abp.BackgroundJobs
{
	public static class JobDataConsts
	{
		public const int MaxTypeNameLength = 512;
		public const int MaxNameLength = 128;
		public const int MaxArgumentsKeyLength = 40;
		public const int MaxArgumentsTypeLength = MaxTypeNameLength;
		public const int MaxConcurrencyKeyLength = JobConcurrencyLockConsts.MaxConcurrencyKeyLength;
		public const int MaxResultTypeLength = MaxTypeNameLength;
	}
}
