using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Localization;
using Isap.CommonCore;
using Isap.CommonCore.Services;
using Microsoft.AspNetCore.Mvc;

namespace Isap.Abp.Extensions
{
	public abstract class ReferenceControllerBase<TEntityDto, TKey, TService, TLocalizationResource>
		: IsapControllerBase<TLocalizationResource>, IReferenceAppService<TEntityDto, TKey>
		where TEntityDto: ICommonEntityDto<TKey>
		where TService: IReferenceAppService<TEntityDto, TKey>
		where TLocalizationResource: ILocalizationResource
	{
		protected TService AppService => this.LazyGetRequiredService<TService>();

		[HttpGet]
		[Route("get")]
		public Task<TEntityDto> Get(TKey id)
		{
			return AppService.Get(id);
		}

		[HttpGet]
		[Route("many")]
		public Task<Dictionary<TKey, TEntityDto>> GetMany(TKey[] idList)
		{
			return AppService.GetMany(idList);
		}

		Task<ResultSet<TEntityDto>> IReferenceAppService<TEntityDto, TKey>.GetPage(int pageNumber, int pageSize, bool countTotal, QueryOptionsDto queryOptions)
		{
			throw new NotSupportedException();
		}

		[HttpPost]
		[Route("page")]
		public Task<ResultSet<TEntityDto>> GetPage(int pageNumber, int pageSize, bool countTotal, List<SpecificationParameters> specifications)
		{
			return AppService.GetPage(pageNumber, pageSize, countTotal, specifications);
		}

		Task<ResultSet<TEntityDto>> IReferenceAppService<TEntityDto, TKey>.GetPage(int pageNumber, int pageSize, bool countTotal,
			ICollection<DataFilterValueDto> filterValues)
		{
			throw new NotSupportedException();
		}

		Task<ResultSet<TEntityDto>> IReferenceAppService<TEntityDto, TKey>.QueryPage(int pageNumber, int pageSize, bool countTotal,
			QueryOptionsDto queryOptions)
		{
			throw new NotSupportedException();
		}
	}

	public abstract class ReferenceControllerBase<TEntityDto, TKey, TService>
		: ReferenceControllerBase<TEntityDto, TKey, TService, AbpExtensionsResource>
		where TEntityDto: ICommonEntityDto<TKey>
		where TService: IReferenceAppService<TEntityDto, TKey>
	{
	}
}
