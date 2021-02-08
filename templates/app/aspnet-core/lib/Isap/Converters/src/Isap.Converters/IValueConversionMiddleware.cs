namespace Isap.Converters
{
	public interface IValueConversionMiddleware: IBasicValueConverterProvider
	{
		IValueConversionMiddleware Next { get; }
	}
}
