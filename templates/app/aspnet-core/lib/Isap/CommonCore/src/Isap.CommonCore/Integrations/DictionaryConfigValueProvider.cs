using System;
using System.Collections.Generic;
using Isap.Converters;

namespace Isap.CommonCore.Integrations
{
	public class DictionaryConfigValueProvider: IConfigValueProvider
	{
		private readonly IValueConverter _converter;
		private readonly IDictionary<string, object> _values;

		public DictionaryConfigValueProvider(IValueConverter converter, IDictionary<string, object> values)
		{
			_converter = converter;
			_values = values;
		}

		public string GetSectionName()
		{
			throw new NotImplementedException();
		}

		public T GetValue<T>(string key, Func<T> getDefaultValue)
		{
			object value;
			if (_values.TryGetValue(key, out value))
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
			throw new NotImplementedException();
		}

		public Dictionary<string, string> GetMap(string key, Func<IConfigValueProvider, KeyValuePair<string, string>> convert = null)
		{
			throw new NotImplementedException();
		}

		public IConfigValueProvider GetValueProvider(string key)
		{
			throw new NotImplementedException();
		}
	}
}
