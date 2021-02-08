using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Isap.Abp.FileStorage.Files;
using Isap.CommonCore.Expressions.Evaluation;
using Isap.CommonCore.Utils;

namespace Isap.Abp.FileStorage.Configurations
{
	[StorageType(StorageTypeName)]
	public class LocalFileStorageProvider: FileStorageProviderBase
	{
		public const string StorageTypeName = "Local";

		public LocalFileStorageProvider(IFileStorageProviderConfiguration config)
			: base(config)
		{
			string baseDir = Config.Config.GetValue("BaseDir", "./App_Data/UploadedFiles");
			LocalPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), baseDir));
		}

		public override bool HasLocalPath => true;
		public override string LocalPath { get; }

		public override async Task SaveFileAsync(FileData fileData, Stream stream)
		{
			Guid fileDataId = fileData.Id == Guid.Empty ? Guid.NewGuid() : fileData.Id;
			
			if (string.IsNullOrEmpty(fileData.Path))
				fileData.Path = await GetFilePath(fileData.TenantId, fileDataId, fileData.FileName, fileData.ContentType, fileData.Length);

			var fileInfo = new FileInfo(Path.Combine(LocalPath, fileData.Path));
			Debug.Assert(fileInfo.Directory != null);
			if (!fileInfo.Directory.Exists)
				fileInfo.Directory.Create();
			using (var fileStream = fileInfo.Create())
				await stream.CopyToAsync(fileStream);
			if (string.IsNullOrEmpty(fileData.Hash))
			{
				MD5 md5 = MD5.Create();
				using (var fileStream = fileInfo.OpenRead())
				{
					byte[] hash = md5.ComputeHash(fileStream);
					fileData.Hash = HexUtility.ByteArrayToString(hash).ToLower();
				}
			}
		}

		protected override Task<string> GetFilePath(int tenantId, Guid id, string fileName, string contentType, long length)
		{
			object result = ExpressionTranslators.Default.Translate(Config.Config.GetValue("RelativePathTemplate", "${tenantId}/${id}"),
				new Dictionary<string, string>
					{
						{ "tenantId", tenantId.ToString("x8") },
						{ "id", id.ToString().ToLower() },
						{ "name", Path.GetFileNameWithoutExtension(fileName) },
						{ "extension", Path.GetExtension(fileName) },
					});
			return Task.FromResult(Convert.ToString(result));
		}

		protected override Task<string> GetFilePath(Guid? tenantId, Guid id, string fileName, string contentType, long length)
		{
			object result = ExpressionTranslators.Default.Translate(Config.Config.GetValue("RelativePathTemplate", "${tenantId}/${id}"),
				new Dictionary<string, string>
					{
						{ "tenantId", tenantId?.ToString("N") },
						{ "id", id.ToString().ToLower() },
						{ "name", Path.GetFileNameWithoutExtension(fileName) },
						{ "extension", Path.GetExtension(fileName) },
					});
			return Task.FromResult(Convert.ToString(result));
		}
	}
}
