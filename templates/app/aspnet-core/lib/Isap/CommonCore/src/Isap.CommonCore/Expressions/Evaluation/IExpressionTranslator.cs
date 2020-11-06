using System.Collections.Generic;

namespace Isap.CommonCore.Expressions.Evaluation
{
	public interface IExpressionTranslator
	{
		string Translate(string input, IExpressionEvaluator evaluator);
		string Translate(string input, IEvaluateExpressionValueProvider valueProvider);
		string Translate(string input, Dictionary<string, string> values);
		string Translate(string input, Dictionary<string, object> values);
	}
}
