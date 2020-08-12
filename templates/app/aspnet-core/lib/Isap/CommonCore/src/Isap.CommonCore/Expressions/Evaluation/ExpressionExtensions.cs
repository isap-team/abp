namespace Isap.CommonCore.Expressions.Evaluation
{
	public static class ExpressionExtensions
	{
		public static string Translate(this string input, IEvaluateExpressionValueProvider valueProvider)
		{
			return ExpressionTranslators.Default.Translate(input, valueProvider);
		}

		public static string Translate(this string input, IExpressionTranslator translator, IEvaluateExpressionValueProvider valueProvider)
		{
			return input == null ? null : translator.Translate(input, valueProvider);
		}
	}
}
