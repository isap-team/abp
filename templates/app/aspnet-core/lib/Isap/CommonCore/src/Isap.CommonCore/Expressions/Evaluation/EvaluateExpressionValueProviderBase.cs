using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Isap.Converters;

namespace Isap.CommonCore.Expressions.Evaluation
{
	public abstract class EvaluateExpressionValueProviderBase<TIntf>: IEvaluateExpressionValueProvider
	{
		protected static class PropertyAccessors
		{
			public static IPropertyGetAccessor<TIntf, TPropertyValue> CreateGetAccessor<TPropertyValue>(Expression<Func<TIntf, TPropertyValue>> expression)
			{
				return new PropertyGetAccessor<TIntf, TPropertyValue>(expression);
			}
		}

		protected EvaluateExpressionValueProviderBase(TIntf entry, IValueConverter converter, IDictionary<string, object> args)
		{
			Entry = entry;
			Converter = converter;
			Arguments = args ?? new Dictionary<string, object>();
		}

		public IValueConverter Converter { get; set; }
		public IDictionary<string, object> Arguments { get; }

		protected abstract Dictionary<string, IPropertyGetAccessor<TIntf>> PropertyAccessorMap { get; }

		public TIntf Entry { get; }

		public bool TryGetValue(string id, out object result)
		{
			if (Arguments.TryGetValue(id, out object value))
			{
				result = value;
				return true;
			}

			if (PropertyAccessorMap.TryGetValue(id, out IPropertyGetAccessor<TIntf> accessor))
			{
				result = accessor.GetValue(Entry);
				return true;
			}

			result = null;
			return false;
		}

		public bool TryGetObject(string id, out object result)
		{
			throw new NotImplementedException();
		}
	}
}
