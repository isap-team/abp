namespace Isap.Abp.BackgroundJobs
{
	public enum JobStateType
	{
		Abandoned = -1,
		New = 0,
		Pending = 1,
		Completed = 2,
		Cancelled = 3,
	}
}
