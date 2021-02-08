using System;
using System.Collections.Generic;
using Isap.Converters;
using Microsoft.Extensions.Configuration;

namespace Isap.Abp.FileStorage.Configurations
{
	public class FileStorageConfiguration: IFileStorageConfiguration
	{
		private readonly Dictionary<string, IFileStorageProviderConfiguration> _providers = new Dictionary<string, IFileStorageProviderConfiguration>();

		public FileStorageConfiguration(IValueConverter converter, IConfigurationSection config)
		{
			foreach (IConfigurationSection providerConfig in config.GetChildren())
			{
				FileStorageProviderConfiguration providerConfiguration = new FileStorageProviderConfiguration(converter, providerConfig);
				_providers.Add(providerConfig.Key, providerConfiguration);
			}
		}

		public IFileStorageProviderConfiguration GetProviderConfig(string providerName)
		{
			return _providers.TryGetValue(providerName, out IFileStorageProviderConfiguration result)
					? result
					: throw new InvalidOperationException($"Can't find file storage provider configuration with name '{providerName}'.")
				;
		}
	}

	public interface IFileStorageConfiguration
	{
		IFileStorageProviderConfiguration GetProviderConfig(string providerName);
	}
}
