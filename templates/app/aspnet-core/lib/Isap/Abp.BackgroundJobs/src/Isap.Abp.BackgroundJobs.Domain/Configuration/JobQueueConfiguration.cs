using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Isap.Abp.Extensions.Clustering;

namespace Isap.Abp.BackgroundJobs.Configuration
{
	public interface IJobQueueConfiguration
	{
		/// <summary>
		///     Наименование очереди.
		/// </summary>
		string Name { get; }

		/// <summary>
		///     True если для каждой ноды в класетере должна создаваться собственная очередь.
		/// </summary>
		bool UseSharding { get; }

		/// <summary>
		///     Интервал ожидания новой задачи в очереди.
		/// </summary>
		int JobPollInterval { get; }

		/// <summary>
		///     Максимальное количество параллеьных потоков, обрабатывающих задачи из очереди.
		/// </summary>
		int MaxThreadCount { get; }

		/// <summary>
		///     Список ролей приложения, разделенных запятыми, для которыми будет обрабатываться очередь.
		/// </summary>
		ICollection<string> ApplicationRoles { get; }

		/// <summary>
		///     Default duration (as seconds) for the first wait on a failure.
		///     Default value: 60 (1 minutes).
		/// </summary>
		int DefaultFirstWaitDuration { get; }

		/// <summary>
		///     Default timeout value (as seconds) for a job before it's abandoned (<see cref="JobData.State" />).
		///     Default value: 172,800 (2 days).
		/// </summary>
		int DefaultTimeout { get; }

		/// <summary>
		///     Default wait factor for execution failures.
		///     This amount is multiplated by last wait time to calculate next wait time.
		///     Default value: 2.0.
		/// </summary>
		double DefaultWaitFactor { get; }
	}

	public class JobQueueConfiguration: IJobQueueConfiguration
	{
		private ICollection<string> _applicationRoles = new List<string>
			{
				AbpClusterNodeOptions.DefaultApplicationRole,
			};

		public string Name { get; set; }
		public bool UseSharding { get; set; } = false;
		public int JobPollInterval { get; set; } = JobQueueConsts.DefaultJobPollInterval;
		public int MaxThreadCount { get; set; } = JobQueueConsts.DefaultMaxThreadCount;

		public string ApplicationRoles
		{
			get => string.Join(",", _applicationRoles);
			set
			{
				IList<string> list = value
					.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
					.DefaultIfEmpty(AbpClusterNodeOptions.DefaultApplicationRole)
					.ToArray();
				_applicationRoles = new ReadOnlyCollection<string>(list);
			}
		}

		ICollection<string> IJobQueueConfiguration.ApplicationRoles => _applicationRoles;

		public int DefaultFirstWaitDuration { get; set; } = JobQueueConsts.DefaultFirstWaitDuration;
		public int DefaultTimeout { get; set; } = JobQueueConsts.DefaultTimeout;
		public double DefaultWaitFactor { get; set; } = JobQueueConsts.DefaultWaitFactor;
	}
}
