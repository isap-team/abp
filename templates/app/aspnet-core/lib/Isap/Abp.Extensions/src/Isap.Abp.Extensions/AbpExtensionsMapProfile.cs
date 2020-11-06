using AutoMapper;
using Isap.Abp.Extensions.DataFilters;
using Isap.Abp.Extensions.Querying;
using Isap.CommonCore.Services;

namespace Isap.Abp.Extensions
{
	public class AbpExtensionsMapProfile: Profile
	{
		public AbpExtensionsMapProfile()
		{
			CreateMap<DataFilterValueDto, DataFilterValue>()
				;
			CreateMap<SortOptionDto, SortOption>()
				;
		}
	}
}
