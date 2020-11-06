using System;
using System.Collections.Concurrent;
using System.Linq;
using Isap.Converters;

namespace Isap.CommonCore.Converters
{
	internal interface IConvertInvoker
	{
		Type TargetType { get; }
		ConvertAttempt TryConvertTo(object source);
	}

	internal interface IConvertInvoker<T>: IConvertInvoker
	{
		new ConvertAttempt<T> TryConvertTo(object source);
	}

	internal class ConvertInvoker<T>: IConvertInvoker<T>
	{
		public Type TargetType => typeof(T);

		public ConvertAttempt<T> TryConvertTo(object source)
		{
			return ConvertExtensionsTemp.TryConvertTo<T>(source);
		}

		ConvertAttempt IConvertInvoker.TryConvertTo(object source)
		{
			return TryConvertTo(source);
		}
	}

	internal static class ConvertInvoker
	{
		private static readonly IConvertInvoker[] _predefinedInvokers =
			{
				new ConvertInvoker<bool>(),
			};

		private static readonly ConcurrentDictionary<Type, IConvertInvoker> _invokers =
			new ConcurrentDictionary<Type, IConvertInvoker>(_predefinedInvokers.ToDictionary(i => i.TargetType));

		public static IConvertInvoker Create(Type targetType)
		{
			Type invokerType = typeof(ConvertInvoker<>).MakeGenericType(targetType);
			return (IConvertInvoker) Activator.CreateInstance(invokerType);
		}

		public static IConvertInvoker GetOrCreateInvoker(Type toType)
		{
			return _invokers.GetOrAdd(toType, Create);
		}
	}
}
