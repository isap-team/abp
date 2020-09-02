using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Isap.CommonCore.Extensions
{
	public static class DictionaryExtensions
	{
		[Obsolete("Use GetOrDefault method with same parameters.")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> collection, TKey key, Func<TKey, TValue> getDefaultValue)
		{
			return GetOrDefault(collection, key, getDefaultValue);
		}

		public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> collection, TKey key, Func<TKey, TValue> getDefaultValue)
		{
			if (!collection.TryGetValue(key, out var result))
				result = (getDefaultValue ?? (k => default))(key);
			return result;
		}

		[Obsolete("Use GetOrDefault method with same parameters.")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> collection, TKey key, Func<TValue> getDefaultValue)
		{
			return GetOrDefault(collection, key, getDefaultValue);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> collection, TKey key, Func<TValue> getDefaultValue)
		{
			return collection.GetOrDefault(key, dummy => (getDefaultValue ?? (() => default))());
		}

		[Obsolete("Use GetOrDefault method with same parameters.")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> collection, TKey key, TValue defaultValue = default)
		{
			return GetOrDefault(collection, key, defaultValue);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> collection, TKey key, TValue defaultValue = default)
		{
			return collection.GetOrDefault(key, dummy => defaultValue);
		}

		[Obsolete("Use GetOrDefault method with same parameters.")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TResult TryGetValue<TKey, TValue, TResult>(this IDictionary<TKey, TValue> collection, TKey key, Func<TValue, TResult> extractResult,
			Func<TKey, TResult> getDefaultValue)
		{
			return GetOrDefault(collection, key, extractResult, getDefaultValue);
		}

		public static TResult GetOrDefault<TKey, TValue, TResult>(this IDictionary<TKey, TValue> collection, TKey key, Func<TValue, TResult> extractResult,
			Func<TKey, TResult> getDefaultValue)
		{
			if (collection.TryGetValue(key, out TValue value))
				return extractResult(value);
			return getDefaultValue(key);
		}

		[Obsolete("Use GetOrDefault method with same parameters.")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TResult TryGetValue<TKey, TValue, TResult>(this IDictionary<TKey, TValue> collection, TKey key, Func<TValue, TResult> extractResult,
			Func<TResult> getDefaultValue)
		{
			return GetOrDefault(collection, key, extractResult, getDefaultValue);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TResult GetOrDefault<TKey, TValue, TResult>(this IDictionary<TKey, TValue> collection, TKey key, Func<TValue, TResult> extractResult,
			Func<TResult> getDefaultValue)
		{
			return collection.GetOrDefault(key, extractResult, dummy => (getDefaultValue ?? (() => default))());
		}

		[Obsolete("Use GetOrDefault method with same parameters.")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TResult TryGetValue<TKey, TValue, TResult>(this IDictionary<TKey, TValue> collection, TKey key, Func<TValue, TResult> extractResult,
			TResult defaultValue = default)
		{
			return GetOrDefault(collection, key, extractResult, defaultValue);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TResult GetOrDefault<TKey, TValue, TResult>(this IDictionary<TKey, TValue> collection, TKey key, Func<TValue, TResult> extractResult,
			TResult defaultValue = default)
		{
			return collection.GetOrDefault(key, extractResult, dummy => defaultValue);
		}

		public static void Assign<TKey, TValue>(this IDictionary<TKey, TValue> destination, IDictionary<TKey, TValue> source)
		{
			foreach (KeyValuePair<TKey, TValue> pair in source)
				destination[pair.Key] = pair.Value;
		}
	}
}
