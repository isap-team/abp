using System;
using Isap.Converters.Extensions;

namespace Isap.Converters
{
	public abstract class ConvertAttempt
	{
		protected ConvertAttempt(bool isSuccess, object result)
		{
			IsSuccess = isSuccess;
			Result = result;
		}

		public bool IsSuccess { get; }
		public object Result { get; }

		public abstract Type ResultType { get; }

		public static ConvertAttempt<T> Success<T>(T result)
		{
			return new ConvertAttempt<T>(true, result);
		}

		public static ConvertAttempt Success(object result)
		{
			if (result == null)
				return new ConvertAttempt<object>(true, null);
			Type attemptType = typeof(ConvertAttempt<>).MakeGenericType(result.GetType());
			return (ConvertAttempt) Activator.CreateInstance(attemptType, true, result);
		}

		public static ConvertAttempt<T> Fail<T>()
		{
			return new ConvertAttempt<T>(false, default(T));
		}

		public static ConvertAttempt Fail(Type type)
		{
			Type attemptType = typeof(ConvertAttempt<>).MakeGenericType(type);
			return (ConvertAttempt) Activator.CreateInstance(attemptType, false, attemptType.GetDefaultValue());
		}

		public ConvertAttempt<T> Cast<T>()
		{
			return this as ConvertAttempt<T> ?? new ConvertAttempt<T>(IsSuccess, (T) Result);
		}
	}

	public class ConvertAttempt<T>: ConvertAttempt
	{
		public ConvertAttempt(bool isSuccess, T result)
			: base(isSuccess, result)
		{
		}

		public new T Result => (T) base.Result;
		public override Type ResultType => typeof(T);

		public T AsDefaultIfNotSuccess(Func<T> getDefaultValue)
		{
			return IsSuccess ? Result : getDefaultValue();
		}

		public T AsDefaultIfNotSuccess(T defaultValue = default(T))
		{
			return IsSuccess ? Result : defaultValue;
		}
	}
}
