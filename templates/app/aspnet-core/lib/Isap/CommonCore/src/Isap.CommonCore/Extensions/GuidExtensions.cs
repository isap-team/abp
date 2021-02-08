using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Isap.CommonCore.Extensions
{
	public static class GuidExtensions
	{
		private static readonly short __guidB = 0;
		private static readonly short __guidC = 0;
		private static readonly byte[] __guidD = Enumerable.Repeat((byte) 0, 8).ToArray();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsEmpty(this Guid value)
		{
			return Equals(value, Guid.Empty);
		}

		public static Guid ToGuid(this int value)
		{
			return new Guid(value, __guidB, __guidC, __guidD);
		}

		public static int ToInt(this Guid value)
		{
			byte[] bytes = value.ToByteArray();
			int result = bytes[3];
			result = (result << 8) + bytes[2];
			result = (result << 8) + bytes[1];
			result = (result << 8) + bytes[0];
			return result;
		}
	}
}
