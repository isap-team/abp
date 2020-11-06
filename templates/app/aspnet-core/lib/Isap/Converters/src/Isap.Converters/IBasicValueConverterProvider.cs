using System;

namespace Isap.Converters
{
	public interface IBasicValueConverterProvider
	{
		IBasicValueConverter GetBasicConverter(Type fromType, Type toType);
	}
}
