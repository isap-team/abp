using System;
using Isap.CommonCore.Integrations;
using Isap.Converters;
using Microsoft.Extensions.Configuration;

namespace Isap.Abp.FileStorage.Configurations
{
	public class FileStorageProviderConfiguration: IFileStorageProviderConfiguration
	{
		public FileStorageProviderConfiguration(IValueConverter converter, IConfigurationSection config)
		{
			StorageType = config[nameof(StorageType)];
			BaseUrl = new Uri(converter.TryConvertTo<string>(config[nameof(BaseUrl)]).AsDefaultIfNotSuccess("/UploadedFiles"), UriKind.RelativeOrAbsolute);
			Config = new ConfigurationSectionValueProvider(converter, config);
		}

		public string StorageType { get; }
		public Uri BaseUrl { get; }
		public IConfigValueProvider Config { get; }
	}

	public interface IFileStorageProviderConfiguration
	{
		string StorageType { get; }
		Uri BaseUrl { get; }
		IConfigValueProvider Config { get; }
	}
}
