namespace Isap.CommonCore.Expressions.Evaluation
{
	public static class ExpressionExtensions
	{
		public static object Translate(this string input, IEvaluateExpressionValueProvider valueProvider)
		{
			return ExpressionTranslators.Default.Translate(input, valueProvider);
		}

		public static object Translate(this string input, IExpressionTranslator translator, IEvaluateExpressionValueProvider valueProvider)
		{
			return input == null ? null : translator.Translate(input, valueProvider);
		}
	}
}
