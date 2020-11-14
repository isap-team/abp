namespace Isap.CommonCore.Expressions.Evaluation
{
	public class ValueProviderExpressionEvaluator: IExpressionEvaluator
	{
		private readonly IEvaluateExpressionValueProvider _provider;

		public ValueProviderExpressionEvaluator(IEvaluateExpressionValueProvider provider)
		{
			_provider = provider;
		}

		public bool TryEvaluate(string expression, out object result)
		{
			return _provider.TryGetValue(expression, out result);
		}
	}
}
