namespace Isap.Converters
{
	public class CachedValueConverterProvider: IValueConverterProvider
	{
		private readonly IValueConverterProvider _baseProvider;
		private readonly IValueConverter _converter;

		public CachedValueConverterProvider(IValueConverterProvider baseProvider, IValueConverter converter)
		{
			_baseProvider = baseProvider;
			_converter = converter;
		}

		public IValueConversionMiddleware TopMiddleware => _baseProvider.TopMiddleware;

		public IValueConverterProvider Use<TMiddleware>()
			where TMiddleware: IValueConversionMiddleware
		{
			return _baseProvider.Use<TMiddleware>();
		}

		public IValueConverter GetConverter()
		{
			return _converter;
		}

		public IValueConverterProvider Clone()
		{
			return new CachedValueConverterProvider(_baseProvider, _converter);
		}
	}
}
