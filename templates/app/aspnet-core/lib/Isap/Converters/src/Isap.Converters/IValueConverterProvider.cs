namespace Isap.Converters
{
	public interface IValueConverterProvider
	{
		IValueConversionMiddleware TopMiddleware { get; }
		IValueConverterProvider Use<TMiddleware>() where TMiddleware: IValueConversionMiddleware;
		IValueConverter GetConverter();
		IValueConverterProvider Clone();
	}
}
