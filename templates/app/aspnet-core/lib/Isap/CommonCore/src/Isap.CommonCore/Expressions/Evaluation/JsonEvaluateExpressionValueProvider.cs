using System;
using System.Linq;
using Isap.CommonCore.Extensions;
using Isap.Converters;
using Newtonsoft.Json.Linq;

namespace Isap.CommonCore.Expressions.Evaluation
{
	public class JsonEvaluateExpressionValueProvider: IEvaluateExpressionValueProvider
	{
		private readonly JObject _targetObject;

		public JsonEvaluateExpressionValueProvider(JObject targetObject)
		{
			_targetObject = targetObject;
			Converter = ValueConverterProviders.Current.GetConverter();
		}

		public IValueConverter Converter { get; set; }

		public bool TryGetValue(string id, out object result)
		{
			string format;
			string[] idItems = id.Split(new[] { ':' }, 2);
			switch (idItems.Length)
			{
				case 1:
					id = idItems[0];
					format = null;
					break;
				case 2:
					id = idItems[0];
					format = idItems[1];
					break;
				default:
					throw new InvalidOperationException();
			}

			return TryGetValue(_targetObject, id.Split('.').ToArraySegment(), format, out result);
		}

		protected bool TryGetValue(JObject targetObject, ArraySegment<string> idPath, string format, out object result)
		{
			if (targetObject.TryGetValue(idPath.First(), out JToken token))
			{
				switch (token)
				{
					case null:
						result = null;
						return true;
					case JObject jObject:
						return TryGetValue(jObject, idPath.ToArraySegment(1), format, out result);
					case JValue jValue:
						return TryConvertToString(jValue, format, out result);
				}
			}

			result = null;
			return false;
		}

		private bool TryConvertToString(JValue value, string format, out object result)
		{
			if (!string.IsNullOrEmpty(format))
			{
				switch (value.Value)
				{
					case string stringValue:
						result = string.Format(format, stringValue);
						return true;
					case DateTime dateTime:
						result = dateTime.ToString(format);
						return true;
				}
			}

			result = value.Value;
			return true;
		}
	}
}
