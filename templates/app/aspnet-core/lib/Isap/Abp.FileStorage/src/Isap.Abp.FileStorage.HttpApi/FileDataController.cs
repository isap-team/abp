using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Isap.Abp.Extensions;
using Isap.Abp.FileStorage.Files;
using Isap.Abp.FileStorage.Localization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace Isap.Abp.FileStorage
{
	[RemoteService]
	[Route("api/file-storage/file")]
	public class FileDataController: BasicControllerBase<FileDataDto, Guid, IFileDataAppService, FileStorageResource>, IFileDataAppService
	{
		[HttpPost]
		[Route("upload")]
		public async Task<List<FileDataDto>> Upload(string fileName, string contentType, byte[] bytes)
		{
			return await AppService.Upload(fileName, contentType, bytes);
		}
	}
}
