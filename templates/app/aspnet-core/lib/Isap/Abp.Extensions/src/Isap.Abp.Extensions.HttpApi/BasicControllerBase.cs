using System.Threading.Tasks;
using Isap.Abp.Extensions.Localization;
using Isap.CommonCore.Services;
using Microsoft.AspNetCore.Mvc;

namespace Isap.Abp.Extensions
{
	public abstract class BasicControllerBase<TEntityDto, TKey, TService, TLocalizationResource>
		: ReferenceControllerBase<TEntityDto, TKey, TService, TLocalizationResource>, IBasicAppService<TEntityDto, TKey>
		where TEntityDto: ICommonEntityDto<TKey>
		where TService: IBasicAppService<TEntityDto, TKey>
		where TLocalizationResource: ILocalizationResource
	{
		[HttpPost]
		[Route("create")]
		public Task<TEntityDto> Create(TEntityDto entry)
		{
			return AppService.Create(entry);
		}

		[HttpPut]
		[Route("update")]
		public Task<TEntityDto> Update(TEntityDto entry)
		{
			return AppService.Update(entry);
		}

		[HttpPost]
		[Route("save")]
		public Task<TEntityDto> Save(TEntityDto entry)
		{
			return AppService.Save(entry);
		}

		[HttpDelete]
		[Route("delete")]
		public Task Delete(TKey id)
		{
			return AppService.Delete(id);
		}

		[HttpPost]
		[Route("undelete")]
		public Task<TEntityDto> Undelete(TKey id)
		{
			return AppService.Undelete(id);
		}
	}

	public abstract class BasicControllerBase<TEntityDto, TKey, TService>
		: BasicControllerBase<TEntityDto, TKey, TService, AbpExtensionsResource>
		where TEntityDto: ICommonEntityDto<TKey>
		where TService: IBasicAppService<TEntityDto, TKey>
	{
	}
}
