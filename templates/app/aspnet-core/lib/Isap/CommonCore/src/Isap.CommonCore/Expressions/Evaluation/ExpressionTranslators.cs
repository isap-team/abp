namespace Isap.CommonCore.Expressions.Evaluation
{
	public static class ExpressionTranslators
	{
		public static readonly IExpressionTranslator Default = new ExpressionTranslator("${", "}");
	}
}
