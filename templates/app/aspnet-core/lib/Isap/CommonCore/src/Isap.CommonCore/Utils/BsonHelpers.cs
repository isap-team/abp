using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace Isap.CommonCore.Utils
{
	public static class BsonHelpers
	{
		public static byte[] ToBson<T>(T value)
		{
			using (MemoryStream ms = new MemoryStream())
			using (BsonDataWriter dataWriter = new BsonDataWriter(ms))
			{
				JsonSerializer serializer = new JsonSerializer();
				serializer.Serialize(dataWriter, value);
				return ms.ToArray();
			}
		}

		public static T FromBson<T>(byte[] data)
		{
			using (MemoryStream ms = new MemoryStream(data))
			using (BsonDataReader reader = new BsonDataReader(ms))
			{
				JsonSerializer serializer = new JsonSerializer();
				return serializer.Deserialize<T>(reader);
			}
		}
	}
}
