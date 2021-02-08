using System.Collections.Generic;

namespace Isap.CommonCore.Intervals
{
	public interface IIntervalValueTypeDescriptor<TValue>
	{
		IComparer<TValue> Comparer { get; }
		TValue MinValue { get; }
		TValue MaxValue { get; }
		TValue GetPrevValue(TValue value);
		TValue GetNextValue(TValue value);
		bool Between(TValue value, TValue fromValue, TValue toValue);
		bool GreaterThen(TValue leftValue, TValue rightValue);
		bool GreaterThenOrEqual(TValue leftValue, TValue rightValue);
		bool LessThen(TValue leftValue, TValue rightValue);
		bool LessThenOrEqual(TValue leftValue, TValue rightValue);
		TValue Normalize(TValue value);
	}
}
