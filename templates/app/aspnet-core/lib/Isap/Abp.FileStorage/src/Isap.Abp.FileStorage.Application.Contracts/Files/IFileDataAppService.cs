using System.Collections.Generic;
using System.Threading.Tasks;
using Isap.CommonCore.Services;

namespace Isap.Abp.FileStorage.Files
{
	public interface IFileDataAppService: IBasicAppService<FileDataDto>
	{
		Task<List<FileDataDto>> Upload(string fileName, string contentType, byte[] bytes);
	}
}
