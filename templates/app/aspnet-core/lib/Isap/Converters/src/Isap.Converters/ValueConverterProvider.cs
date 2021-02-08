using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Isap.Converters
{
	public class ValueConverterProvider: IValueConverterProvider, IBasicValueConverterProvider
	{
		private readonly ConcurrentDictionary<Type, Guid> _typeIdMap = new ConcurrentDictionary<Type, Guid>();
		private readonly ConcurrentDictionary<string, IBasicValueConverter> _converterMap;

		public ValueConverterProvider(ValueConverterProvider baseProvider)
		{
			_converterMap = new ConcurrentDictionary<string, IBasicValueConverter>(baseProvider._converterMap);
			TopMiddleware = baseProvider.TopMiddleware;
		}

		public ValueConverterProvider()
		{
			_converterMap = new ConcurrentDictionary<string, IBasicValueConverter>();
			TopMiddleware = new FailoverValueConversionMiddleware();
		}

		public IValueConversionMiddleware TopMiddleware { get; private set; }

		public IBasicValueConverter GetBasicConverter(Type fromType, Type toType)
		{
			string key = $"{GetTypeKey(fromType)}:{GetTypeKey(toType)}";
			return _converterMap.GetOrAdd(key, k => TopMiddleware.GetBasicConverter(fromType, toType));
		}

		public IValueConverterProvider Use<TMiddleware>()
			where TMiddleware: IValueConversionMiddleware
		{
			TopMiddleware = (IValueConversionMiddleware) Activator.CreateInstance(typeof(TMiddleware), TopMiddleware);
			_converterMap.Clear();
			return this;
		}

		public IValueConverter GetConverter()
		{
			return new ValueConverter(this);
		}

		public IValueConverterProvider Clone()
		{
			return new ValueConverterProvider(this);
		}

		private string GetTypeKey(Type type)
		{
			Guid typeId = type.GUID;
			if (Equals(typeId, Guid.Empty))
				typeId = _typeIdMap.GetOrAdd(type, _ => Guid.NewGuid());

			if (type.IsGenericType)
			{
				string argumentsKey = string.Join(",", type.GetGenericArguments().Select(GetTypeKey));
				return $"{typeId:N}<{argumentsKey}>";
			}

			return typeId.ToString("N");
		}
	}
}
