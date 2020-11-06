using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Isap.CommonCore.Extensions
{
	public static class StringExtensions
	{
		private static readonly Regex __whitespaceRegex = new Regex(@"\s");

		private static readonly Regex __splitByChunksRegex = new Regex(@"\s+", RegexOptions.Compiled | RegexOptions.Singleline);

		public static string RemoveWhitespaces(this string value)
		{
			return __whitespaceRegex.Replace(value, string.Empty);
		}

		public static string WithPrefixIfNotNullOrEmpty(this string value, [NotNull] string prefix)
		{
			if (prefix == null) throw new ArgumentNullException(nameof(prefix));
			if (String.IsNullOrEmpty(value))
				return value;
			return prefix + value;
		}

		public static string EnsurePrefix(this string value, [NotNull] string prefix)
		{
			if (prefix == null) throw new ArgumentNullException(nameof(prefix));
			if (String.IsNullOrEmpty(value) || value.StartsWith(prefix))
				return value;
			return prefix + value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string AsEmptyIfNull(this string value)
		{
			return value ?? String.Empty;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string AsNullIfEmpty(this string value)
		{
			return value == String.Empty ? null : value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string AsNullIf(this string value, string valueToCompare)
		{
			return value == valueToCompare ? null : value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string AsEmptyIf(this string value, string valueToCompare)
		{
			return value == valueToCompare ? string.Empty : value;
		}

		public static string ToStringOrNull(this object obj)
		{
			if (obj == null)
				return null;
			return Convert.ToString(obj);
		}

		public static (bool isTrue, string value, Func<string, T> then) IfNullThen<T>(this string value, Func<T> then)
		{
			return (isTrue: value == null, value: value, then: v => then());
		}

		public static (bool isTrue, string value, Func<string, T> then) IfNullThen<T>(this string value, Func<string, T> then)
		{
			return (isTrue: value == null, value: value, then: then);
		}

		public static (bool isTrue, string value, Func<string, T> then) IfNullThen<T>(this string value, T thenValue = default(T))
		{
			return (isTrue: value == null, value: value, then: v => thenValue);
		}

		public static (bool isTrue, string value, Func<string, T> then) IfNotNullThen<T>(this string value, Func<T> then)
		{
			return (isTrue: value != null, value: value, then: v => then());
		}

		public static (bool isTrue, string value, Func<string, T> then) IfNotNullThen<T>(this string value, Func<string, T> then)
		{
			return (isTrue: value != null, value: value, then: then);
		}

		public static (bool isTrue, string value, Func<string, T> then) IfNotNullThen<T>(this string value, T thenValue = default(T))
		{
			return (isTrue: value != null, value: value, then: v => thenValue);
		}

		public static T Else<T>(this (bool isTrue, string value, Func<string, T> then) condition, Func<T> @else)
		{
			return condition.isTrue ? condition.then(condition.value) : @else();
		}

		public static T Else<T>(this (bool isTrue, string value, Func<string, T> then) condition, Func<string, T> @else)
		{
			return condition.isTrue ? condition.then(condition.value) : @else(condition.value);
		}

		public static T Else<T>(this (bool isTrue, string value, Func<string, T> then) condition, T elseValue)
		{
			return condition.isTrue ? condition.then(condition.value) : elseValue;
		}

		public static T ElseDefault<T>(this (bool isTrue, string value, Func<string, T> then) condition)
		{
			return condition.isTrue ? condition.then(condition.value) : default(T);
		}

		public static string TruncStart(this string value, int maxLength, string truncatedValueMarker = "...")
		{
			if (value == null)
				return null;
			if (maxLength <= 0)
				return string.Empty;
			if (value.Length <= maxLength)
				return value;
			if (truncatedValueMarker.Length >= maxLength)
				return truncatedValueMarker.Substring(truncatedValueMarker.Length - maxLength, maxLength);
			return truncatedValueMarker + value.Substring(value.Length - maxLength + truncatedValueMarker.Length, maxLength - truncatedValueMarker.Length);
		}

		public static string TruncEnd(this string value, int maxLength, string truncatedValueMarker = "...")
		{
			if (value == null)
				return null;
			if (maxLength <= 0)
				return string.Empty;
			if (value.Length <= maxLength)
				return value;
			if (truncatedValueMarker.Length >= maxLength)
				return truncatedValueMarker.Substring(0, Math.Min(maxLength, value.Length));
			return value.Substring(0, maxLength - truncatedValueMarker.Length) + truncatedValueMarker;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void IfNotNullOrEmpty(this string value, Action<string> action)
		{
			if (!string.IsNullOrEmpty(value))
				action(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void IfNotNullOrWhiteSpace(this string value, Action<string> action)
		{
			if (!string.IsNullOrWhiteSpace(value))
				action(value);
		}

		public static IEnumerable<string> Chunk(this string value, int chunkSize)
		{
			string result = "";
			foreach (string s in __splitByChunksRegex.Split(value))
			{
				string temp = result + " " + s;
				if (temp.Length > chunkSize && !string.IsNullOrEmpty(result))
				{
					yield return result;
					temp = s;
				}

				result = temp;
			}

			if (!string.IsNullOrEmpty(result))
				yield return result;
		}

		public static string ToCamalCase(this string value)
		{
			if (value == null)
				return null;
			return value.Substring(0, 1).ToLower() + value.Substring(1);
		}

		public static string ToPascalCase(this string value)
		{
			if (value == null)
				return null;
			return value.Substring(0, 1).ToUpper() + value.Substring(1);
		}

		public static string Md5Hash(this string input)
		{
			using (var md5 = MD5.Create())
			{
				byte[] result = md5.ComputeHash(Encoding.ASCII.GetBytes(input));
				return BitConverter.ToString(result).Replace("-", "").ToLower();
			}
		}
	}
}
