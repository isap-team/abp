using System;
using Isap.Abp.Extensions.Caching;
using Volo.Abp.DependencyInjection;

namespace Isap.Abp.FileStorage.Files
{
	public interface IFileDataCache: IDistributedEntityCache<FileDataDto, Guid>
	{
	}

	public class FileDataCache: DistributedEntityCacheBase<FileDataDto, FileDataDto, Guid, IFileDataAppService>, IFileDataCache, ISingletonDependency
	{
		public FileDataCache(IFileDataAppService appService)
			: base(appService)
		{
		}

		protected override string EntityName => "FileData";
	}
}
