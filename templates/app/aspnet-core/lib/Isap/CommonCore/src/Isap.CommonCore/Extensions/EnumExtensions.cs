using System;
using System.Linq;

namespace Isap.CommonCore.Extensions
{
	public static class EnumExtensions
	{
		public static string Encode<T>(this T value, params string[] encodeValues)
			where T: struct
		{
			if (!typeof(T).IsEnum)
				throw new InvalidOperationException("Encode method can be used with enum types only.");
			return Enum.GetValues(typeof(T))
				.Cast<T>()
				.Zip(encodeValues, Tuple.Create)
				.Where(tuple => Equals(tuple.Item1, value))
				.Select(tuple => tuple.Item2)
				.FirstOrDefault();
		}
	}
}
