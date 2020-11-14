using System.Collections.Generic;

namespace Isap.CommonCore.Expressions.Evaluation
{
	public interface IExpressionTranslator
	{
		object Translate(string input, IExpressionEvaluator evaluator);
		object Translate(string input, IEvaluateExpressionValueProvider valueProvider);
		object Translate(string input, Dictionary<string, string> values);
		object Translate(string input, Dictionary<string, object> values);
	}
}
