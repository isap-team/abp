using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Isap.Abp.Extensions.DataFilters
{
	public static class DataFilterOptionsExtensions
	{
		public static Dictionary<string, object> Deserialize(JToken options)
		{
			return options?.ToObject<Dictionary<string, object>>();
		}

		public static Dictionary<string, object> Deserialize(string options)
		{
			return JsonConvert.DeserializeObject<Dictionary<string, object>>(options, new DataFilterValueJsonConverter());
		}

		public static Dictionary<string, object> Replace(this Dictionary<string, object> targetOptions, Dictionary<string, object> replaceOptions)
		{
			var result = new Dictionary<string, object>(targetOptions, StringComparer.OrdinalIgnoreCase);
			foreach (KeyValuePair<string, object> pair in replaceOptions)
				result[pair.Key] = pair.Value;
			return result;
		}

		public static string Serialize<T>(this T options)
			where T: DataFilterOptions
		{
			return JsonConvert.SerializeObject(options, new DataFilterValueJsonConverter());
		}
	}
}
