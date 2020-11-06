namespace Isap.CommonCore.Expressions.Evaluation
{
	public interface IExpressionEvaluator
	{
		bool TryEvaluate(string expression, out string result);
	}
}
