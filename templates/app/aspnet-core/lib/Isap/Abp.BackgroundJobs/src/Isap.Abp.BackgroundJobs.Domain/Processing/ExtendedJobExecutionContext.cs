using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using Volo.Abp.BackgroundJobs;

namespace Isap.Abp.BackgroundJobs.Processing
{
	public class ExtendedJobExecutionContext: JobExecutionContext
	{
		public ExtendedJobExecutionContext(IServiceProvider serviceProvider, Type jobType, object jobArgs, ILogger logger, CancellationToken cancellationToken)
			: base(serviceProvider, jobType, jobArgs)
		{
			Logger = logger;
			CancellationToken = cancellationToken;
		}

		public ILogger Logger { get; }
		public CancellationToken CancellationToken { get; }
		public object Result { get; set; }
	}
}
