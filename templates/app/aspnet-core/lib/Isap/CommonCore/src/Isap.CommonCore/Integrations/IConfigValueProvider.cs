using System;
using System.Collections.Generic;

namespace Isap.CommonCore.Integrations
{
	public interface IConfigValueProvider
	{
		string GetSectionName();
		T GetValue<T>(string key, Func<T> getDefaultValue);
		T GetValue<T>(string key, T defaultValue = default(T));
		List<T> GetList<T>(string key, Func<IConfigValueProvider, T> convert);
		IConfigValueProvider GetValueProvider(string key);
	}
}
