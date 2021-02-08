using System;
using System.IO;
using System.Threading.Tasks;
using Isap.Abp.FileStorage.Files;

namespace Isap.Abp.FileStorage.Configurations
{
	public abstract class FileStorageProviderBase: IFileStorageProvider
	{
		protected FileStorageProviderBase(IFileStorageProviderConfiguration config)
		{
			Config = config;
		}

		protected IFileStorageProviderConfiguration Config { get; }

		public abstract bool HasLocalPath { get; }
		public abstract string LocalPath { get; }
		public Uri BaseUrl => Config.BaseUrl;

		public Task<FileData> CreateFileData(string name, int tenantId, string fileName, string contentType, long length)
		{
			throw new NotImplementedException();
		}

		public virtual async Task<FileData> CreateFileData(string name, Guid? tenantId, string fileName, string contentType, long length)
		{
			Guid id = Guid.NewGuid();
			return new FileData(id)
				{
					TenantId = tenantId,
					FileName = fileName,
					ContentType = contentType,
					Length = length,
					Name = name,
					Path = await GetFilePath(tenantId, id, fileName, contentType, length),
				};
		}

		public abstract Task SaveFileAsync(FileData fileData, Stream stream);

		protected abstract Task<string> GetFilePath(int tenantId, Guid id, string contentType, string s, long length);
		protected abstract Task<string> GetFilePath(Guid? tenantId, Guid id, string contentType, string s, long length);
	}
}
