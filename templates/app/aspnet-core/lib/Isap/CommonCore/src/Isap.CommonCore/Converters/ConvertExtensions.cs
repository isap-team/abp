using System;
using Isap.Converters;

namespace Isap.CommonCore.Converters
{
	[Obsolete("Use Isap.Converters.IValueConverter")]
	public static class ConvertExtensions
	{
		[Obsolete("Use Isap.Converters.IValueConverter.TryConvertTo(object, Type)")]
		public static ConvertAttempt TryConvertTo(this object source, Type toType)
		{
			return ConvertExtensionsTemp.TryConvertTo(source, toType);
		}

		[Obsolete("Use Isap.Converters.IValueConverter.TryConvertTo<T>(object)")]
		public static ConvertAttempt<T> TryConvertTo<T>(this object source)
		{
			return ConvertExtensionsTemp.TryConvertTo<T>(source);
		}

		[Obsolete("Use Isap.Converters.IValueConverter.ConvertTo<T>(object)")]
		public static T ConvertTo<T>(this object source)
		{
			return ConvertExtensionsTemp.ConvertTo<T>(source);
		}

		[Obsolete("Use Isap.Converters.IValueConverter.ConvertTo(object, Type)")]
		public static object ConvertTo(this object source, Type toType)
		{
			return ConvertExtensionsTemp.ConvertTo(source, toType);
		}
	}
}
