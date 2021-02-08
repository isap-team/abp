using System;
using Isap.CommonCore.Services;

namespace Isap.Abp.BackgroundJobs
{
	public interface IBackgroundProcessingEntity: ICommonEntity<Guid>
	{
		DateTime CreationTime { get; }
	}
}
