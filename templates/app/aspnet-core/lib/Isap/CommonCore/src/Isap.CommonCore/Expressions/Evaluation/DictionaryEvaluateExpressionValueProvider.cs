using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Isap.CommonCore.Extensions;
using Isap.Converters;

namespace Isap.CommonCore.Expressions.Evaluation
{
	public class DictionaryEvaluateExpressionValueProvider: IEvaluateExpressionValueProvider
	{
		private static readonly Regex _idRegex = new Regex(@"(?<id>[^\:]+)(\:(?<pos>\d+)(\,(?<len>\d+))?)?", RegexOptions.Compiled | RegexOptions.Singleline);

		private readonly Dictionary<string, string> _values;

		public DictionaryEvaluateExpressionValueProvider(Dictionary<string, string> values)
		{
			_values = values;
		}

		public DictionaryEvaluateExpressionValueProvider(Dictionary<string, object> values)
		{
			IValueConverter converter = ValueConverterProviders.Default.GetConverter();
			_values = values.ToDictionary(pair => pair.Key, pair => converter.ConvertTo<string>(pair.Value), values.Comparer);
		}

		public bool TryGetValue(string id, out object result)
		{
			Match match = _idRegex.Match(id);
			if (!match.Success)
				throw new InvalidOperationException($"Invalid id format '{id}'.");
			string idStr = match.Groups["id"].Value;
			int position = match.Groups["pos"].As(g => g.Success ? int.Parse(g.Value) : 0);
			int length = match.Groups["len"].As(g => g.Success ? int.Parse(g.Value) : -1);

			if (_values.TryGetValue(idStr, out string value))
			{
				result = string.IsNullOrEmpty(value)
						? null
						: (length < 0 ? value.Substring(position) : value.Substring(position, length))
					;
				return true;
			}

			result = null;
			return false;
		}
	}
}
