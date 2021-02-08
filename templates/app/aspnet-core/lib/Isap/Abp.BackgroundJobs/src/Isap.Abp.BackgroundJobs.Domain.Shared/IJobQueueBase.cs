namespace Isap.Abp.BackgroundJobs
{
	public interface IJobQueueBase: IBackgroundProcessingEntity
	{
		string Name { get; }
	}
}
