using System;
using System.Linq;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.BackgroundJobs;

namespace Isap.Abp.BackgroundJobs
{
	[AttributeUsage(AttributeTargets.Class)]
	public class BackgroundJobAttribute: Attribute, IBackgroundJobNameProvider, IBackgroundJobQueueNameProvider
	{
		public BackgroundJobAttribute([NotNull] string name)
		{
			Name = Check.NotNullOrWhiteSpace(name, nameof(name));
		}

		public string Name { get; }

		public string QueueName { get; set; }

		[ItemNotNull]
		public static string GetName<TJobArgs>()
		{
			return GetName(typeof(TJobArgs));
		}

		[ItemNotNull]
		public static string GetName([NotNull] Type jobArgsType)
		{
			Check.NotNull(jobArgsType, nameof(jobArgsType));

			return jobArgsType
					.GetCustomAttributes(true)
					.OfType<IBackgroundJobNameProvider>()
					.FirstOrDefault()
					?.Name
				?? jobArgsType.FullName;
		}

		[ItemCanBeNull]
		public static string GetQueueName<TJobArgs>()
		{
			return GetQueueName(typeof(TJobArgs));
		}

		[ItemCanBeNull]
		public static string GetQueueName([NotNull] Type jobArgsType)
		{
			Check.NotNull(jobArgsType, nameof(jobArgsType));

			return jobArgsType
				.GetCustomAttributes(true)
				.OfType<IBackgroundJobQueueNameProvider>()
				.FirstOrDefault()
				?.QueueName;
		}
	}
}
