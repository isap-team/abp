using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Isap.CommonCore.API
{
	/// <summary>
	///     API enum helpers.
	/// </summary>
	public static class ApiEnumValueHelpers
	{
		[Flags]
		private enum IntFlags
		{
		}

		[Flags]
		private enum UIntFlags: uint
		{
		}

		[Flags]
		private enum LongFlags: long
		{
		}

		[Flags]
		private enum ULongFlags: ulong
		{
		}

		private static readonly ConcurrentDictionary<Type, Dictionary<string, object>> __enumValueMaps =
			new ConcurrentDictionary<Type, Dictionary<string, object>>();

		private static readonly ConcurrentDictionary<Type, Dictionary<object, string>> __enumValueReverseMaps =
			new ConcurrentDictionary<Type, Dictionary<object, string>>();

		private static readonly Dictionary<Type, Func<IEnumerable<object>, object>> __aggregateByUnderlyingTypeMap =
			new Dictionary<Type, Func<IEnumerable<object>, object>>
				{
					{ typeof(int), flags => flags.Aggregate((IntFlags) 0, (sum, value) => sum | (IntFlags) value) },
					{ typeof(uint), flags => flags.Aggregate((UIntFlags) 0U, (sum, value) => sum | (UIntFlags) value) },
					{ typeof(long), flags => flags.Aggregate((LongFlags) 0L, (sum, value) => sum | (LongFlags) value) },
					{ typeof(ulong), flags => flags.Aggregate((ULongFlags) 0UL, (sum, value) => sum | (ULongFlags) value) },
				};

		private static readonly Dictionary<Type, Func<object, object, bool>> __isSetByUnderlyingTypeMap = new Dictionary<Type, Func<object, object, bool>>
			{
				{ typeof(int), (flags, value) => ((IntFlags) flags & (IntFlags) value) == (IntFlags) value },
				{ typeof(uint), (flags, value) => ((UIntFlags) flags & (UIntFlags) value) == (UIntFlags) value },
				{ typeof(long), (flags, value) => ((LongFlags) flags & (LongFlags) value) == (LongFlags) value },
				{ typeof(ulong), (flags, value) => ((ULongFlags) flags & (ULongFlags) value) == (ULongFlags) value },
			};

		private static Dictionary<string, object> GetEnumValueMap([NotNull] Type enumType)
		{
			if (enumType == null) throw new ArgumentNullException(nameof(enumType));
			if (!enumType.IsEnum) throw new ArgumentException($"Enum type should be specified.", nameof(enumType));

			return __enumValueMaps.GetOrAdd(enumType, CreateEnumValueMap);
		}

		private static Dictionary<string, object> CreateEnumValueMap(Type enumType)
		{
			FieldInfo[] fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);

			Dictionary<string, object> result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

			foreach (FieldInfo field in fields)
			{
				if (field.GetCustomAttribute<ApiEnumIgnoreAttribute>() == null)
				{
					string fieldName = field.GetCustomAttribute<ApiEnumValueAttribute>()?.Value ?? field.Name;
					result.Add(fieldName, field.GetValue(null));
				}
			}

			return result;
		}

		private static Dictionary<object, string> GetEnumValueReverseMap([NotNull] Type enumType)
		{
			if (enumType == null) throw new ArgumentNullException(nameof(enumType));
			if (!enumType.IsEnum) throw new ArgumentException($"Enum type should be specified.", nameof(enumType));

			return __enumValueReverseMaps.GetOrAdd(enumType, CreateEnumValueReverseMap);
		}

		private static Dictionary<object, string> CreateEnumValueReverseMap(Type enumType)
		{
			Dictionary<string, object> map = GetEnumValueMap(enumType);
			return map
				.GroupBy(pair => pair.Value)
				.ToDictionary(g => g.Key, g => g.First().Key);
		}

		/// <summary>
		///     Converts enum values from API enum value representation.
		/// </summary>
		/// <typeparam name="TEnum">Enum type.</typeparam>
		/// <param name="values">Values to convert.</param>
		/// <returns>Converted values.</returns>
		public static IEnumerable<TEnum> ToEnumItems<TEnum>(this IEnumerable<string> values)
			where TEnum: struct
		{
			Dictionary<string, object> valueMap = GetEnumValueMap(typeof(TEnum));

			TEnum Convert(string value)
			{
				return valueMap.TryGetValue(value, out object result) ? (TEnum) result : throw new InvalidOperationException();
			}

			foreach (string value in values)
				yield return Convert(value);
		}

		/// <summary>
		///     Converts enum values from API enum value representation.
		/// </summary>
		/// <typeparam name="TEnum">Enum flags type.</typeparam>
		/// <param name="values">Values to convert.</param>
		/// <returns>Converted values.</returns>
		public static TEnum ToEnumFlags<TEnum>(this IEnumerable<string> values)
			where TEnum: struct
		{
			if (!typeof(TEnum).GetCustomAttributes<FlagsAttribute>().Any())
				throw new InvalidOperationException($"Enum type '{typeof(TEnum).FullName}' should be marked with {nameof(FlagsAttribute)}.");

			Type underlyingType = Enum.GetUnderlyingType(typeof(TEnum));
			if (!__aggregateByUnderlyingTypeMap.TryGetValue(underlyingType, out var aggregate))
				throw new InvalidOperationException();

			return (TEnum) aggregate(values.ToEnumItems<TEnum>().Cast<object>());
		}

		/// <summary>
		///     Convert enum flags to enumerable of API enum values.
		/// </summary>
		/// <typeparam name="TEnum">Enum flags type.</typeparam>
		/// <param name="flags">Values to convert.</param>
		/// <returns>Converted values.</returns>
		public static IEnumerable<string> ToApiEnumValueList<TEnum>(this TEnum flags)
		{
			if (!typeof(TEnum).GetCustomAttributes<FlagsAttribute>().Any())
				throw new InvalidOperationException($"Enum type '{typeof(TEnum).FullName}' should be marked with {nameof(FlagsAttribute)}.");

			Dictionary<object, string> valueMap = GetEnumValueReverseMap(typeof(TEnum));

			Type underlyingType = Enum.GetUnderlyingType(typeof(TEnum));
			if (!__isSetByUnderlyingTypeMap.TryGetValue(underlyingType, out var isSet))
				throw new InvalidOperationException();

			foreach (object value in Enum.GetValues(typeof(TEnum)))
			{
				if (isSet(flags, value))
					if (valueMap.TryGetValue(value, out string fieldName))
						yield return fieldName;
			}
		}
	}
}
