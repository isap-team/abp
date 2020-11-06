using System;

namespace Isap.CommonCore.Extensions
{
	public static class ArrayExtensions
	{
		public static ArraySegment<T> ToArraySegment<T>(this T[] array, int offset, int count)
		{
			if (array == null)
				return new ArraySegment<T>();
			return new ArraySegment<T>(array, offset, count);
		}

		public static ArraySegment<T> ToArraySegment<T>(this T[] array, int offset)
		{
			if (array == null)
				return new ArraySegment<T>();
			int count = array.Length - offset;
			return new ArraySegment<T>(array, offset, count);
		}

		public static ArraySegment<T> ToArraySegment<T>(this T[] array)
		{
			if (array == null)
				return new ArraySegment<T>();
			return new ArraySegment<T>(array, 0, array.Length);
		}

		public static ArraySegment<T> ToArraySegment<T>(this ArraySegment<T> array, int offset, int count)
		{
			if (array.Array == null)
				return new ArraySegment<T>();
			int newOffset = array.Offset + offset;
			return new ArraySegment<T>(array.Array, newOffset, count);
		}

		public static ArraySegment<T> ToArraySegment<T>(this ArraySegment<T> array, int offset)
		{
			if (array.Array == null)
				return new ArraySegment<T>();
			int newOffset = array.Offset + offset;
			int newCount = array.Count - offset;
			return new ArraySegment<T>(array.Array, newOffset, newCount);
		}
	}
}
