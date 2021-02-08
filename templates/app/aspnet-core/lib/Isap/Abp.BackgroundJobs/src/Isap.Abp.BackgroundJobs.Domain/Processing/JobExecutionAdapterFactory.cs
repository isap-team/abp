using System;
using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy.Internal;
using Volo.Abp;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;

namespace Isap.Abp.BackgroundJobs.Processing
{
	public interface IJobExecutionAdapterFactory
	{
		IJobExecutionAdapter Create(Type jobType);
	}

	public class JobExecutionAdapterFactory: IJobExecutionAdapterFactory, ITransientDependency
	{
		private static readonly List<Type> _jobGenericTypes = new List<Type>
			{
				typeof(IBackgroundJob<>),
				typeof(IAsyncBackgroundJob<>),
				typeof(IExtendedAsyncBackgroundJob<>),
			};

		public IJobExecutionAdapter Create(Type jobType)
		{
			Tuple<Type, Type> jobInterface = jobType.GetAllInterfaces()
				.Where(type => type.IsGenericType)
				.Select(type => Tuple.Create(type, type.GetGenericTypeDefinition()))
				.Select(tuple => Tuple.Create(tuple, _jobGenericTypes.IndexOf(tuple.Item2)))
				.Where(tuple => tuple.Item2 >= 0)
				.OrderByDescending(tuple => tuple.Item2)
				.Select(tuple => tuple.Item1)
				.FirstOrDefault();

			if (jobInterface == null)
			{
				throw new AbpException(
					$"Given job type does not implement {string.Join(" or ", _jobGenericTypes.Select(i => i.Name))}. The job type was: {jobType}.");
			}

			Type concreteType = null;
			if (jobInterface.Item2 == typeof(IExtendedAsyncBackgroundJob<>))
				concreteType = typeof(ExtendedBackgroundJobExecutionAdapter<>).MakeGenericType(jobInterface.Item1.GetGenericArguments()[0]);

			else if (jobInterface.Item2 == typeof(IAsyncBackgroundJob<>))
				concreteType = typeof(AsyncBackgroundJobExecutionAdapter<>).MakeGenericType(jobInterface.Item1.GetGenericArguments()[0]);

			else if (jobInterface.Item2 == typeof(IBackgroundJob<>))
				concreteType = typeof(BackgroundJobExecutionAdapter<>).MakeGenericType(jobInterface.Item1.GetGenericArguments()[0]);

			return (IJobExecutionAdapter) Activator.CreateInstance(concreteType ?? throw new NotSupportedException());
		}
	}
}
