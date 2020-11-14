using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Isap.Converters.Extensions
{
	public static class TypeExtensions
	{
		private interface IDefaultValueProvider
		{
			Type Type { get; }
			object DefaultValue { get; }
		}

		private class DefaultValueProvider<T>: IDefaultValueProvider
		{
			public Type Type => typeof(T);
			public object DefaultValue => default(T);
		}

		private static readonly List<IDefaultValueProvider> _defaultValueProviders = new List<IDefaultValueProvider>
			{
				new DefaultValueProvider<bool>(),
				new DefaultValueProvider<long>(),
				new DefaultValueProvider<ulong>(),
				new DefaultValueProvider<int>(),
				new DefaultValueProvider<uint>(),
				new DefaultValueProvider<short>(),
				new DefaultValueProvider<ushort>(),
				new DefaultValueProvider<sbyte>(),
				new DefaultValueProvider<byte>(),
				new DefaultValueProvider<char>(),
				new DefaultValueProvider<Guid>(),
			};

		private static readonly ConcurrentDictionary<Type, IDefaultValueProvider> _defaultValueProviderMap =
			new ConcurrentDictionary<Type, IDefaultValueProvider>(_defaultValueProviders.ToDictionary(p => p.Type));

		public static object GetDefaultValue(this Type type)
		{
			if (type.GetTypeInfo().IsClass)
				return null;
			IDefaultValueProvider provider = _defaultValueProviderMap.GetOrAdd(type,
				t => (IDefaultValueProvider) Activator.CreateInstance(typeof(DefaultValueProvider<>).MakeGenericType(t)));
			return provider.DefaultValue;
		}

		public static bool IsDefaultValue(this object value)
		{
			if (value == null)
				return true;
			Type type = value.GetType();
			return Equals(value, type.GetDefaultValue());
		}

		public static T IfDefault<T>(this T value, Func<T> mapDefaultValue)
		{
			return Equals(value, default(T)) ? mapDefaultValue() : value;
		}

		public static T IfDefault<T>(this T value, T defaultValue)
		{
			return IfDefault(value, () => defaultValue);
		}

		public static async Task<TNewValue> IfNotDefault<TOldValue, TNewValue>(this TOldValue value, Func<TOldValue, Task<TNewValue>> mapValue)
		{
			return !Equals(value, default(TOldValue)) ? await mapValue(value) : default;
		}

		public static TNewValue IfNotDefault<TOldValue, TNewValue>(this TOldValue value, Func<TOldValue, TNewValue> mapValue)
		{
			return !Equals(value, default(TOldValue)) ? mapValue(value) : default;
		}

		public static TNewValue IfNotDefault<TOldValue, TNewValue>(this TOldValue value, TNewValue newValue)
		{
			return IfNotDefault(value, oldValue => newValue);
		}

		public static bool AreEquals(this IValueConverter converter, object first, object second)
		{
			if (first == null)
				return second == null;

			ConvertAttempt attempt = converter.TryConvertTo(first.GetType(), second);
			return attempt.IsSuccess && Equals(first, attempt.Result);
		}
	}
}
