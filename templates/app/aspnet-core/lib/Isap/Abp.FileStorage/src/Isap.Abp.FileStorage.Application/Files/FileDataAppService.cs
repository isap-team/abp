using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Isap.Abp.Extensions.MultiTenancy;
using Isap.Abp.FileStorage.Configurations;
using Isap.Abp.FileStorage.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Isap.Abp.FileStorage.Files
{
	[Authorize]
	public class FileDataAppService
		: FileStorageAppServiceBase<FileDataDto, IFileData, FileData, Guid, IFileDataManager>, IFileDataAppService
	{
		protected IFileStorageProvider FileStorageProvider => LazyGetRequiredService<IFileStorageProvider>();

		protected override string PermissionGroupName => FileStoragePermissions.GroupName;

		protected override Task<Guid?> GetOwnerId(FileDataDto entry)
		{
			return Task.FromResult((Guid?) null);
		}

		public async Task<List<FileDataDto>> Upload(string fileName, string contentType, byte[] bytes)
		{
			Guid tenantId = CurrentTenant.Id ?? IsapMultiTenancyConsts.DefaultTenant.Id;
			//using (AbpSession.Use(tenantId, 2))
			using (CurrentTenant.Change(tenantId))
			{
				var result = new List<FileDataDto>();

				FileData fileData = await FileStorageProvider.CreateFileData(Path.GetFileNameWithoutExtension(fileName),
					tenantId, fileName, contentType, bytes.Length);

				Logger.LogDebug(
					$"Saving uploaded file '{fileData.Path}' (original file name: {fileData.FileName}, length: {fileData.Length}, hash: {fileData.Hash}).");

				await using (var stream = new MemoryStream(bytes))
				{
					await FileStorageProvider.SaveFileAsync(fileData, stream);
				}

				FileDataDto file = ObjectMapper.Map<FileData, FileDataDto>(fileData);
				FileDataDto savedFileDataDto = await Save(file);
				result.Add(savedFileDataDto);

				return result;
			}
		}
	}
}
