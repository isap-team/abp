using System;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;

namespace Isap.Abp.BackgroundJobs.Processing
{
	public interface IJobQueueProcessorFactory
	{
		IJobQueueProcessor Create(IJobQueueBase jobQueue);
	}

	public class JobQueueProcessorFactory: IJobQueueProcessorFactory, ISingletonDependency
	{
		public JobQueueProcessorFactory(IServiceProvider serviceProvider)
		{
			ServiceProvider = serviceProvider;
		}

		protected IServiceProvider ServiceProvider { get; }

		public IJobQueueProcessor Create(IJobQueueBase jobQueue)
		{
			JobQueueProcessor result = ServiceProvider.GetRequiredService<JobQueueProcessor>();
			result.Attach(jobQueue);
			return result;
		}
	}
}
