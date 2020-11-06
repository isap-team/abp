using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Isap.Converters;
using Microsoft.Extensions.Configuration;

namespace Isap.CommonCore.Integrations
{
	public class ConfigurationSectionValueProvider: IConfigValueProvider
	{
		private readonly IValueConverter _converter;
		private readonly IConfigurationSection _config;

		public ConfigurationSectionValueProvider(IValueConverter converter, IConfigurationSection config)
		{
			_converter = converter;
			_config = config;
		}

		public string GetSectionName()
		{
			return _config.Key;
		}

		public T GetValue<T>(string key, Func<T> getDefaultValue)
		{
			getDefaultValue = getDefaultValue ?? (() => default(T));
			Type targetType = typeof(T);
			if (targetType == typeof(string))
			{
				string strValue;
				return TryGetValue(key, out strValue) ? (T) (object) strValue : getDefaultValue();
			}

			if (typeof(IEnumerable).IsAssignableFrom(targetType))
			{
				List<string> tempValues = GetConfigValueList(key);
				if (tempValues.Count == 0)
					return getDefaultValue();

				if (targetType.IsArray)
				{
					ConvertAttempt attempt = _converter.TryConvertTo(targetType, tempValues);
					return attempt.IsSuccess ? (T) attempt.Result : getDefaultValue();
				}

				if (targetType.IsGenericType)
				{
					Type genericType = targetType.GetGenericTypeDefinition();
					if (genericType == typeof(List<>) || genericType == typeof(IEnumerable<>) || genericType == typeof(ICollection<>)
						|| genericType == typeof(IList<>))
					{
						ConvertAttempt attempt = _converter.TryConvertTo(targetType, tempValues);
						return attempt.IsSuccess ? (T) attempt.Result : getDefaultValue();
					}
				}

				if (targetType == typeof(IEnumerable) || targetType == typeof(ICollection) || targetType == typeof(IList))
				{
					ConvertAttempt attempt = _converter.TryConvertTo(targetType, tempValues);
					return attempt.IsSuccess ? (T) attempt.Result : getDefaultValue();
				}

				throw new NotSupportedException();
			}

			string value = _config[key];
			if (value != null)
			{
				ConvertAttempt<T> attempt = _converter.TryConvertTo<T>(value);
				if (attempt.IsSuccess)
					return attempt.Result;
			}

			return getDefaultValue();
		}

		public T GetValue<T>(string key, T defaultValue = default(T))
		{
			return GetValue(key, () => defaultValue);
		}

		public List<T> GetList<T>(string key, Func<IConfigValueProvider, T> convert)
		{
			return _config.GetSection(key).GetChildren()
				.Select(section =>
					{
						var provider = new ConfigurationSectionValueProvider(_converter, section);
						return convert(provider);
					})
				.ToList();
		}

		public IConfigValueProvider GetValueProvider(string key)
		{
			return new ConfigurationSectionValueProvider(_converter, _config.GetSection(key));
		}

		private bool TryGetValue(string key, out string value)
		{
			value = _config[key];
			return value != null;
		}

		private List<string> GetConfigValueList(string key)
		{
			List<string> tempValues = new List<string>();
			for (int i = 0; i < int.MaxValue; i++)
			{
				string value = _config[$"{key}:{i}"];
				if (value == null)
					break;
				tempValues.Add(value);
			}

			return tempValues;
		}
	}
}
