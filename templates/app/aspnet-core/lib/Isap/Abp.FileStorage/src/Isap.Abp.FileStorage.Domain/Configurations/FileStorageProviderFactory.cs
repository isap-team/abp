using System;

namespace Isap.Abp.FileStorage.Configurations
{
	public class FileStorageProviderFactory
		: IFileStorageProviderFactory
	{
		public IFileStorageProvider Create(IFileStorageProviderConfiguration providerConfig)
		{
			if (providerConfig.StorageType.Equals(LocalFileStorageProvider.StorageTypeName, StringComparison.OrdinalIgnoreCase))
				return new LocalFileStorageProvider(providerConfig);

			throw new NotImplementedException();
		}
	}

	public interface IFileStorageProviderFactory
	{
		IFileStorageProvider Create(IFileStorageProviderConfiguration config);
	}
}
