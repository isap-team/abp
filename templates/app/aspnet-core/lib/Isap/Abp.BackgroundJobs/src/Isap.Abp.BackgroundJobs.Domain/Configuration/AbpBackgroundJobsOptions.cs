using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Isap.CommonCore.Extensions;

namespace Isap.Abp.BackgroundJobs.Configuration
{
	public class AbpBackgroundJobsOptions: IBackgroundProcessingConfiguration
	{
		public static readonly TimeSpan DefaultProcessorInactiveTimeout = TimeSpan.FromSeconds(60D);
		public static readonly TimeSpan DefaultObsoleteJobRemovingTimeout = TimeSpan.FromHours(72D);

		private static readonly ICollection<IJobQueueConfiguration> _defaultQueuesConfiguration = new ReadOnlyCollection<IJobQueueConfiguration>(
			new IJobQueueConfiguration[]
				{
					new JobQueueConfiguration
						{
							Name = JobQueueConsts.DefaultQueueName,
						}
				}
		);

		public bool IsEnabled { get; set; } = true;
		public string DefaultQueueName { get; set; } = JobQueueConsts.DefaultQueueName;
		public TimeSpan ProcessorInactiveTimeout { get; set; } = DefaultProcessorInactiveTimeout;
		public TimeSpan ObsoleteJobRemovingTimeout { get; set; } = DefaultObsoleteJobRemovingTimeout;

		public List<JobQueueConfiguration> Queues { get; set; }
		ICollection<IJobQueueConfiguration> IBackgroundProcessingConfiguration.Queues =>
			Queues?.Cast<IJobQueueConfiguration>().ToReadOnlyCollection() ?? _defaultQueuesConfiguration;
	}
}
