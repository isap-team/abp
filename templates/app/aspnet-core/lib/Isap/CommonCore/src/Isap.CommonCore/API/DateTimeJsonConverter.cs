using System;
using Isap.CommonCore.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Isap.CommonCore.API
{
	public class DateTimeJsonConverter: JsonConverter
	{
		public DateTimeJsonConverter(bool asUnixTimestamp, string format)
		{
			AsUnixTimestamp = asUnixTimestamp;
			Format = format;
		}

		public bool AsUnixTimestamp { get; }
		public string Format { get; }

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			switch (value)
			{
				case DateTime dt:
					JToken token = AsUnixTimestamp
							? JToken.FromObject(dt.ToUnixTimestamp())
							: JToken.FromObject(dt.ToString(Format))
						;
					token.WriteTo(writer);
					break;
				default:
					throw new NotSupportedException();
			}
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			switch (reader.TokenType)
			{
				case JsonToken.Integer:
					int time = Convert.ToInt32(reader.Value);
					return time.FromUnixTimestamp();
				case JsonToken.Date:
					return Convert.ToDateTime(reader.Value);
				case JsonToken.String:
					return Convert.ToDateTime(reader.Value);
				case JsonToken.Null:
					if (objectType.IsClass)
						return null;
					if (objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Nullable<>))
						return null;
					throw new NotSupportedException();
				default:
					throw new NotSupportedException();
			}
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(DateTime)
				|| objectType == typeof(DateTime?)
				;
		}
	}
}
