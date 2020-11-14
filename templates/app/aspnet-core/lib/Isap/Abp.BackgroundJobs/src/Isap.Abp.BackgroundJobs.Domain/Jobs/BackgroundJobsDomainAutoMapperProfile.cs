using AutoMapper;
using Isap.Abp.BackgroundJobs.Queues;

namespace Isap.Abp.BackgroundJobs.Jobs
{
	public class BackgroundJobsDomainAutoMapperProfile: Profile
	{
		public BackgroundJobsDomainAutoMapperProfile()
		{
			CreateMap<IJobQueue, JobQueueDto>();

			/*
			CreateMap<BackgroundJobInfo, JobData>()
				.ConstructUsing(x => new JobData(x.Id))
				.ForMember(e => e.TenantId, opt => opt.MapFrom((src, dest, member, res) => res.GetService<ICurrentTenant>()?.Id))
				.ForMember(e => e.Name, opt => opt.MapFrom(src => src.JobName))
				.ForMember(e => e.Arguments, opt => opt.MapFrom(src => src.JobArgs))
				.ForMember(e => e.State, opt => opt.MapFrom(src => src.IsAbandoned ? JobStateType.Abandoned : JobStateType.Pending))
				.Ignore(e => e.ConcurrencyStamp)
				.Ignore(e => e.ExtraProperties)
				.Ignore(e => e.QueueId)
				.Ignore(e => e.Queue)
				.Ignore(e => e.ArgumentsKey)
				.Ignore(e => e.ArgumentsType)
				.Ignore(e => e.ConcurrencyKey)
				.Ignore(e => e.LockId)
				.Ignore(e => e.LockTime)
				.Ignore(e => e.Result)
				;

			CreateMap<IJobData, BackgroundJobInfo>()
				.ForMember(e => e.JobName, opt => opt.MapFrom(src => src.Name))
				.ForMember(e => e.JobArgs, opt => opt.MapFrom(src => src.Arguments))
				.ForMember(e => e.IsAbandoned, opt => opt.MapFrom(src => src.State == JobStateType.Abandoned))
				;
			*/
		}
	}
}
