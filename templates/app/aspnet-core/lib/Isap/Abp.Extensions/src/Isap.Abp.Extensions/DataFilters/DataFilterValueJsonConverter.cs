using System;
using System.Globalization;
using Newtonsoft.Json;

namespace Isap.Abp.Extensions.DataFilters
{
	public class DataFilterValueJsonConverter: JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			switch (value)
			{
				case decimal decimalValue:
					writer.WriteToken(JsonToken.String, decimalValue.ToString(CultureInfo.InvariantCulture));
					break;
				default:
					throw new NotSupportedException();
			}
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotSupportedException();
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(decimal);
		}
	}
}
