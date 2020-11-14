using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Isap.Converters.BasicConverters
{
	public static class JValueConverter
	{
		public static readonly Dictionary<JTokenType, Type> TypeMap = new Dictionary<JTokenType, Type>
			{
				{ JTokenType.Guid, typeof(Guid) },
				{ JTokenType.String, typeof(string) },
				{ JTokenType.Boolean, typeof(bool) },
			};
	}

	public class JValueConverter<T>: BasicValueConverterBase<JValue, T>
	{
		public override ConvertAttempt<T> TryConvert(IBasicValueConverterProvider converterProvider, JValue value)
		{
			if (JValueConverter.TypeMap.TryGetValue(value.Type, out Type valueType))
			{
				IBasicValueConverter converter = converterProvider.GetBasicConverter(valueType, typeof(T));
				if (converter == null)
					return ConvertAttempt.Fail<T>();
				return converter.TryConvert(converterProvider, value.Value).Cast<T>();
			}

			throw new NotImplementedException();
		}
	}
}
