namespace Isap.Abp.BackgroundJobs
{
	public static class JobQueueConsts
	{
		public const string DefaultQueueName = "Default";
		public const int DefaultJobPollInterval = 5000;
		public const int DefaultMaxThreadCount = 1;
		public const int DefaultFirstWaitDuration = 60;
		public const int DefaultTimeout = 172800;
		public const double DefaultWaitFactor = 2.0D;

		public const int MaxNameLength = 128;
	}
}
