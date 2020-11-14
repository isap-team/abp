using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Web;
using Isap.Converters;
using JetBrains.Annotations;

namespace Isap.CommonCore.Extensions
{
	public static class UrlHelpers
	{
		public static string ToQueryString(this IEnumerable<KeyValuePair<string, string>> query)
		{
			return string.Join("&", query.Select(pair => WebUtility.UrlEncode(pair.Key) + '=' + WebUtility.UrlEncode(pair.Value)))
				;
		}

		public static string ToQueryStringParam(this KeyValuePair<string, string> pair)
		{
			return WebUtility.UrlEncode(pair.Key) + '=' + WebUtility.UrlEncode(pair.Value);
		}

		public static string ToQueryString(this IEnumerable<KeyValuePair<string, object>> query, IValueConverter converter = null)
		{
			converter = converter ?? ValueConverterProviders.Default.GetConverter();
			return string.Join("&", query.Select(pair => ToQueryStringParam(pair, converter)));
		}

		public static string ToQueryStringParam(this KeyValuePair<string, object> pair, IValueConverter converter = null)
		{
			converter = converter ?? ValueConverterProviders.Default.GetConverter();
			return ToQueryStringParam(new KeyValuePair<string, string>(pair.Key, converter.ConvertTo<string>(pair.Value)));
		}

		public static (Uri uri, string lastSegment) RemoveLastSegment(this Uri uri)
		{
			if (uri == null) return (null, null);
			return (new Uri($"{uri.Scheme}://{uri.Authority}{string.Join(string.Empty, uri.Segments.Take(uri.Segments.Length - 1))}"), uri.Segments.Last());
		}

		public static string UriChangeScheme(this string uri, string schema)
		{
			int idx = uri.IndexOf("://", StringComparison.Ordinal);
			if (idx < 0)
				throw new InvalidOperationException();
			return schema + uri.Substring(idx);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Uri UriChangeScheme(this Uri uri, string schema)
		{
			return new Uri(UriChangeScheme(uri.ToString(), schema), UriKind.Absolute);
		}

		public static string UriChangeScheme(this string uri, Dictionary<string, string> schemaChangeMap)
		{
			int idx = uri.IndexOf("://", StringComparison.Ordinal);
			if (idx < 0)
				throw new InvalidOperationException();
			string oldScheme = uri.Substring(0, idx);
			string newScheme;
			if (schemaChangeMap.TryGetValue(oldScheme, out newScheme))
				return newScheme + uri.Substring(idx);
			return uri;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Uri UriChangeScheme(this Uri uri, Dictionary<string, string> schemaChangeMap)
		{
			return new Uri(UriChangeScheme(uri.ToString(), schemaChangeMap), UriKind.Absolute);
		}

		public static string SetQueryString<TRequest>(this string url, TRequest request, bool replaceUrlParams = true)
		{
			return SetQueryString(new Uri(url, UriKind.RelativeOrAbsolute), request, replaceUrlParams).ToString();
		}

		public static Uri SetQueryString<TRequest>(this Uri url, TRequest request, bool replaceUrlParams = true)
		{
			return SetQueryString(url, request.AsNameToObjectMap(), replaceUrlParams);
		}

		public static Uri SetQueryString(this Uri url, Dictionary<string, object> queryString, bool replaceUrlParams = true)
		{
			string[] urlItems = url.PathAndQuery.Split(new[] { '?' }, 2, StringSplitOptions.RemoveEmptyEntries);
			string path;
			List<KeyValuePair<string, string>> query = new List<KeyValuePair<string, string>>();
			switch (urlItems.Length)
			{
				case 1:
					path = urlItems[0];
					break;
				case 2:
					path = urlItems[0];
					query.AddRange(TryParseQueryString(urlItems[1]));
					break;
				default:
					throw new InvalidOperationException();
			}

			IValueConverter converter = ValueConverterProviders.Default.GetConverter();
			foreach (KeyValuePair<string, object> pair in queryString)
			{
				switch (pair.Value)
				{
					case string value:
						query.Add(new KeyValuePair<string, string>(pair.Key, value));
						break;
					case IEnumerable enumerable:
						foreach (object value in enumerable)
							query.Add(new KeyValuePair<string, string>(pair.Key, converter.TryConvertTo<string>(value).AsDefaultIfNotSuccess()));
						break;
					default:
						query.Add(new KeyValuePair<string, string>(pair.Key, converter.TryConvertTo<string>(pair.Value).AsDefaultIfNotSuccess()));
						break;
				}
			}

			if (replaceUrlParams)
			{
				query = query.GroupBy(pair => pair.Key)
					.Select(g => new KeyValuePair<string, string>(g.Key, g.Select(pair => pair.Value).Last()))
					.ToList();
			}

			string newQueryString = query.ToQueryString().WithPrefixIfNotNullOrEmpty("?");
			string result = $"{url.Scheme}://{url.Authority}{path}{newQueryString}";
			return new Uri(result);
		}

		public static Uri SetQueryString(this Uri url, string paramName, object paramValue)
		{
			var queryString = new Dictionary<string, object>
				{
					{ paramName, paramValue },
				};
			return SetQueryString(url, queryString);
		}

		public static IEnumerable<KeyValuePair<string, string>> TryParseQueryString(this string queryString)
		{
			if (string.IsNullOrEmpty(queryString))
				return Enumerable.Empty<KeyValuePair<string, string>>();

			string[] urlItems = queryString.Split(new[] { '?' }, 2, StringSplitOptions.RemoveEmptyEntries);
			switch (urlItems.Length)
			{
				case 1:
					queryString = urlItems[0];
					break;
				case 2:
					queryString = urlItems[1];
					break;
				default:
					throw new InvalidOperationException();
			}

			return queryString.Split('&').Select(ParseQueryParameter);
		}

		private static KeyValuePair<string, string> ParseQueryParameter([NotNull] string queryParameter)
		{
			if (queryParameter == null) throw new ArgumentNullException(nameof(queryParameter));
			string[] items = queryParameter.Split(new[] { '=' }, 2);
			switch (items.Length)
			{
				case 2:
					return new KeyValuePair<string, string>(items[0], HttpUtility.UrlDecode(items[1]));
				case 1:
					return new KeyValuePair<string, string>(items[0], null);
				default:
					throw new InvalidOperationException();
			}
		}
	}
}
