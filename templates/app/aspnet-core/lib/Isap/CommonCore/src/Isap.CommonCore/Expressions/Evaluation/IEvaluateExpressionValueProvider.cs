namespace Isap.CommonCore.Expressions.Evaluation
{
	public interface IEvaluateExpressionValueProvider
	{
		bool TryGetValue(string id, out object result);
	}
}
