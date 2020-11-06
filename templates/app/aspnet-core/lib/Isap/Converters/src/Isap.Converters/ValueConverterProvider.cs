using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Isap.Converters
{
	public class ValueConverterProvider: IValueConverterProvider, IBasicValueConverterProvider
	{
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
			if (type.IsGenericType)
			{
				string argumentsKey = string.Join(",", type.GetGenericArguments().Select(GetTypeKey));
				return $"{type.GUID:N}<{argumentsKey}>";
			}

			return type.GUID.ToString("N");
		}
	}
}
