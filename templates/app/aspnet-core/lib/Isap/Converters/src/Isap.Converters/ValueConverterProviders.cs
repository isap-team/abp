using System;
using Isap.Converters.Middlewares;

namespace Isap.Converters
{
	public static class ValueConverterProviders
	{
		private static readonly AsyncLocalStackContainer<IValueConverterProvider> _convertersStackContainer =
			new AsyncLocalStackContainer<IValueConverterProvider>(() => Default);

		public static readonly IValueConverterProvider Default = new ValueConverterProvider()
			.Use<NullableValueConversionMiddleware>()
			.Use<JTokenToValueConversionMiddleware>()
			.Use<ToEnumConversionMiddleware>()
			.Use<StringToValueTypeConversionMiddleware>()
			.Use<ToStringConversionMiddleware>()
			.Use<EnumerableValueConversionMiddleware>()
			.Use<CommonValueConversionMiddleware>();

		public static IValueConverterProvider Current => _convertersStackContainer.Current;

		public static IDisposable Use(IValueConverter converter)
		{
			return _convertersStackContainer.Use(baseProvider => new CachedValueConverterProvider(baseProvider, converter));
		}
	}
}
