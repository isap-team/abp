namespace Isap.Abp.BackgroundJobs
{
	public interface IBackgroundJobQueueNameProvider
	{
		string QueueName { get; }
	}
}
