using System.Text.Json;

namespace Isap.CommonCore.Extensions
{
	public static class JsonExtensions
	{
		public static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true,
			};

		public static T ToObject<T>(this JsonElement element)
		{
			var json = element.GetRawText();
			return JsonSerializer.Deserialize<T>(json, DefaultJsonSerializerOptions);
		}

		public static T ToObject<T>(this JsonDocument document)
		{
			var json = document.RootElement.GetRawText();
			return JsonSerializer.Deserialize<T>(json, DefaultJsonSerializerOptions);
		}
	}
}
