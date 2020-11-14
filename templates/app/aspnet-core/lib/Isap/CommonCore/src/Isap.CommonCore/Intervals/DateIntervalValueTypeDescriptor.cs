using System;
using System.Collections.Generic;

namespace Isap.CommonCore.Intervals
{
	public class DateIntervalValueTypeDescriptor: IIntervalValueTypeDescriptor<DateTime>
	{
		public DateIntervalValueTypeDescriptor(IComparer<DateTime> comparer = null)
		{
			Comparer = comparer ?? Comparer<DateTime>.Default;
			MinValue = DateTime.MinValue.Date;
			MaxValue = DateTime.MaxValue.Date;
		}

		public IComparer<DateTime> Comparer { get; }

		public DateTime MinValue { get; }
		public DateTime MaxValue { get; }

		public DateTime GetPrevValue(DateTime value)
		{
			return value.Date.AddDays(-1D);
		}

		public DateTime GetNextValue(DateTime value)
		{
			return value.Date.AddDays(1D);
		}

		public bool Between(DateTime value, DateTime fromValue, DateTime toValue)
		{
			return Comparer.Compare(value, fromValue) >= 0 && Comparer.Compare(value, toValue) <= 0;
		}

		public bool GreaterThen(DateTime leftValue, DateTime rightValue)
		{
			return leftValue > rightValue;
		}

		public bool GreaterThenOrEqual(DateTime leftValue, DateTime rightValue)
		{
			return leftValue >= rightValue;
		}

		public bool LessThen(DateTime leftValue, DateTime rightValue)
		{
			return leftValue < rightValue;
		}

		public bool LessThenOrEqual(DateTime leftValue, DateTime rightValue)
		{
			return leftValue <= rightValue;
		}

		public DateTime Normalize(DateTime value)
		{
			return value.Date;
		}
	}
}
