using System;
using System.Threading.Tasks;
using Isap.Abp.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ExceptionHandling;

namespace Isap.Abp.BackgroundJobs.Processing
{
	[Dependency(ReplaceServices = true)]
	[ExposeServices(typeof(IBackgroundJobExecuter))]
	public class ExtendedBackgroundJobExecutor: BackgroundJobExecuter
	{
		public ExtendedBackgroundJobExecutor(IOptions<AbpBackgroundJobOptions> options)
			: base(options)
		{
		}

		public IJobExecutionAdapterFactory JobExecutionAdapterFactory { get; set; }
		public IShutdownCancellationTokenProvider ShutdownCancellationTokenProvider { get; set; }

		public override async Task ExecuteAsync(JobExecutionContext context)
		{
			object job = context.ServiceProvider.GetService(context.JobType);
			if (job == null)
			{
				throw new AbpException("The job type is not registered to DI: " + context.JobType);
			}

			IJobExecutionAdapter executionAdapter = JobExecutionAdapterFactory.Create(context.JobType);

			try
			{
				if (context is ExtendedJobExecutionContext extContext)
				{
					extContext.Result = await executionAdapter.ExecuteAsync(job, context.JobArgs, extContext.CancellationToken);
				}
				else
				{
					await executionAdapter.ExecuteAsync(job, context.JobArgs, ShutdownCancellationTokenProvider.CancellationToken);
				}
			}
			catch (Exception ex)
			{
				Logger.LogException(ex);

				await context.ServiceProvider
					.GetRequiredService<IExceptionNotifier>()
					.NotifyAsync(new ExceptionNotificationContext(ex));

				throw new BackgroundJobExecutionException("A background job execution is failed. See inner exception for details.", ex)
					{
						JobType = context.JobType.AssemblyQualifiedName,
						JobArgs = context.JobArgs
					};
			}
		}
	}
}
