using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Isap.CommonCore.Extensions
{
	public static class EmbeddedResourceExtensions
	{
		public static T ReadEmbeddedResource<T>(this Assembly assembly, string @namespace, string name, Func<StreamReader, T> read)
		{
			string resourceName = $"{@namespace}.{name}";
			using (Stream stream = assembly.GetManifestResourceStream(resourceName))
			{
				if (stream == null)
					throw new InvalidOperationException($"Can't find embedded resource with name '{resourceName}'.");
				using (var reader = new StreamReader(stream))
					return read(reader);
			}
		}

		public static async Task<T> ReadEmbeddedResourceAsync<T>(this Assembly assembly, string @namespace, string name, Func<StreamReader, Task<T>> read)
		{
			string resourceName = $"{@namespace}.{name}";
			using (Stream stream = assembly.GetManifestResourceStream(resourceName))
			{
				if (stream == null)
					throw new InvalidOperationException($"Can't find embedded resource with name '{resourceName}'.");
				using (var reader = new StreamReader(stream))
					return await read(reader);
			}
		}

		public static string ReadEmbeddedResourceAsString(this Assembly assembly, string @namespace, string name)
		{
			return ReadEmbeddedResource(assembly, @namespace, name, reader => reader.ReadToEnd());
		}

		public static Task<string> ReadEmbeddedResourceAsStringAsync(this Assembly assembly, string @namespace, string name)
		{
			return ReadEmbeddedResourceAsync(assembly, @namespace, name, reader => reader.ReadToEndAsync());
		}

		public static string ReadEmbeddedResourceAsString(this Assembly assembly, Type namespaceType, string name)
		{
			return ReadEmbeddedResourceAsString(assembly, namespaceType.Namespace, name);
		}

		public static Task<string> ReadEmbeddedResourceAsStringAsync(this Assembly assembly, Type namespaceType, string name)
		{
			return ReadEmbeddedResourceAsStringAsync(assembly, namespaceType.Namespace, name);
		}

		public static string ReadEmbeddedResourceAsString(this Type namespaceType, string name)
		{
			return ReadEmbeddedResourceAsString(namespaceType.Assembly, namespaceType.Namespace, name);
		}

		public static Task<string> ReadEmbeddedResourceAsStringAsync(this Type namespaceType, string name)
		{
			return ReadEmbeddedResourceAsStringAsync(namespaceType.Assembly, namespaceType.Namespace, name);
		}

		public static TObject ReadEmbeddedResourceAsJsonObject<TObject>(this Assembly assembly, string @namespace, string name)
		{
			string json = ReadEmbeddedResourceAsString(assembly, @namespace, name);
			return JsonConvert.DeserializeObject<TObject>(json);
		}

		public static async Task<TObject> ReadEmbeddedResourceAsJsonObjectAsync<TObject>(this Assembly assembly, string @namespace, string name)
		{
			string json = await ReadEmbeddedResourceAsStringAsync(assembly, @namespace, name);
			return JsonConvert.DeserializeObject<TObject>(json);
		}

		public static TObject ReadEmbeddedResourceAsJsonObject<TObject>(this Assembly assembly, Type namespaceType, string name)
		{
			return ReadEmbeddedResourceAsJsonObject<TObject>(assembly, namespaceType.Namespace, name);
		}

		public static Task<TObject> ReadEmbeddedResourceAsJsonObjectAsync<TObject>(this Assembly assembly, Type namespaceType, string name)
		{
			return ReadEmbeddedResourceAsJsonObjectAsync<TObject>(assembly, namespaceType.Namespace, name);
		}

		public static TObject ReadEmbeddedResourceAsJsonObject<TObject>(this Type namespaceType, string name)
		{
			return ReadEmbeddedResourceAsJsonObject<TObject>(namespaceType.Assembly, namespaceType.Namespace, name);
		}

		public static Task<TObject> ReadEmbeddedResourceAsJsonObjectAsync<TObject>(this Type namespaceType, string name)
		{
			return ReadEmbeddedResourceAsJsonObjectAsync<TObject>(namespaceType.Assembly, namespaceType.Namespace, name);
		}
	}
}
