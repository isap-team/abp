using Isap.Abp.Extensions.Domain;
using Isap.Abp.Extensions.Services;
using Isap.Abp.FileStorage.Localization;
using Isap.CommonCore.Services;
using Volo.Abp.Domain.Entities;

namespace Isap.Abp.FileStorage
{
	public abstract class FileStorageAppServiceBase<TEntityDto, TIntf, TImpl, TKey, TDomainManager>
		: BasicAppServiceBase<TEntityDto, TIntf, TImpl, TKey, TDomainManager>
		where TEntityDto: class, ICommonEntityDto<TKey>
		where TIntf: class, ICommonEntity<TKey>
		where TImpl: class, TIntf, IEntity<TKey>
		where TDomainManager: class, IDomainManager<TIntf, TImpl, TKey>
	{
		protected FileStorageAppServiceBase()
		{
			LocalizationResource = typeof(FileStorageResource);
			ObjectMapperContext = typeof(IsapFileStorageApplicationModule);
		}
	}
}
