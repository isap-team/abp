using System;
using System.IO;
using System.Threading.Tasks;
using Isap.Abp.FileStorage.Files;

namespace Isap.Abp.FileStorage.Configurations
{
	public interface IFileStorageProvider
	{
		bool HasLocalPath { get; }
		string LocalPath { get; }
		Uri BaseUrl { get; }
		Task<FileData> CreateFileData(string name, int tenantId, string fileName, string contentType, long length);
		Task<FileData> CreateFileData(string name, Guid? tenantId, string fileName, string contentType, long length);
		Task SaveFileAsync(FileData fileData, Stream stream);
	}
}
