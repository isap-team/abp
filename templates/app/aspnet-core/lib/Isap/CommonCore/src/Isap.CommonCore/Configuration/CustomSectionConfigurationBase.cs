using System;
using Isap.CommonCore.Integrations;
using Isap.Converters;
using Microsoft.Extensions.Configuration;

namespace Isap.CommonCore.Configuration
{
	public abstract class CustomSectionConfigurationBase
	{
		private readonly ConfigurationSectionValueProvider _valueProvider;

		protected CustomSectionConfigurationBase(ConfigurationSectionValueProvider valueProvider)
		{
			_valueProvider = valueProvider;
		}

		protected CustomSectionConfigurationBase(IValueConverter converter, IConfigurationSection config)
			: this(new ConfigurationSectionValueProvider(converter, config))
		{
		}

		public T GetValue<T>(string key, Func<T> getDefaultValue)
		{
			if (_valueProvider == null)
				return default(T);
			return _valueProvider.GetValue(key, getDefaultValue);
		}

		public T GetValue<T>(string key, T defaultValue = default(T))
		{
			return GetValue(key, () => defaultValue);
		}
	}
}
