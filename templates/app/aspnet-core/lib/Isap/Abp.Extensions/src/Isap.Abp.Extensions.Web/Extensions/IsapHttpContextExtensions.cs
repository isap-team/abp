using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Isap.Abp.Extensions.Web.Extensions
{
	public static class IsapHttpContextExtensions
	{
		private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
			{
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
			};

		public static T TryGetObject<T>(this ISession session, string key, Func<T> getDefault)
		{
			string json = session.GetString(key);
			if (string.IsNullOrEmpty(json))
			{
				T @default = getDefault();
				SetObject(session, key, @default);
				return @default;
			}

			return JsonConvert.DeserializeObject<T>(json, _jsonSerializerSettings);
		}

		public static T TryGetObject<T>(this ISession session, Func<T> getDefault)
		{
			return TryGetObject(session, typeof(T).FullName, getDefault);
		}

		public static async Task<T> TryGetObjectAsync<T>(this ISession session, string key, Func<Task<T>> getDefault)
		{
			string json = session.GetString(key);
			if (string.IsNullOrEmpty(json))
			{
				T @default = await getDefault();
				SetObject(session, key, @default);
				return @default;
			}

			return JsonConvert.DeserializeObject<T>(json, _jsonSerializerSettings);
		}

		public static Task<T> TryGetObjectAsync<T>(this ISession session, Func<Task<T>> getDefault)
		{
			return TryGetObjectAsync(session, typeof(T).FullName, getDefault);
		}

		public static T TryGetObject<T>(this ISession session, string key, T defaultValue = default)
		{
			return TryGetObject(session, key, () => defaultValue);
		}

		public static T TryGetObject<T>(this ISession session, T defaultValue = default)
		{
			return TryGetObject(session, typeof(T).FullName, () => defaultValue);
		}

		public static T GetObject<T>(this ISession session, string key)
		{
			T result = TryGetObject(session, key, default(T));
			if (Equals(result, default))
				throw new InvalidOperationException("Can't get object from session.");
			return result;
		}

		public static T GetObject<T>(this ISession session)
		{
			return GetObject<T>(session, typeof(T).FullName);
		}

		public static void SetObject<T>(this ISession session, string key, T value)
		{
			if (Equals(value, default))
				session.Remove(key);
			else
				session.SetString(key, JsonConvert.SerializeObject(value, _jsonSerializerSettings));
		}
	}
}
