using System;
using System.ComponentModel.DataAnnotations;
using Isap.CommonCore.Services;
using Volo.Abp.Application.Dtos;

namespace Isap.Abp.BackgroundJobs
{
	public class JobQueueDto: EntityDto<Guid>, IJobQueueBase
	{
		public DateTime CreationTime { get; set; }

		[Required]
		[StringLength(JobQueueConsts.MaxNameLength)]
		public string Name { get; set; }

		object ICommonEntity.GetId() => Id;
	}
}
