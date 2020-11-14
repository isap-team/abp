using System.Collections;
using System.Collections.Generic;

namespace Isap.CommonCore.Expressions.Evaluation
{
	public class ExpressionEvaluatorCollection: IExpressionEvaluator, IEnumerable<IExpressionEvaluator>
	{
		private readonly List<IExpressionEvaluator> _evaluators = new List<IExpressionEvaluator>();

		public bool TryEvaluate(string expression, out object result)
		{
			result = null;
			foreach (IExpressionEvaluator evaluator in _evaluators)
			{
				if (evaluator.TryEvaluate(expression, out result))
					return true;
			}

			return false;
		}

		public void Add(IExpressionEvaluator evaluator)
		{
			_evaluators.Add(evaluator);
		}

		#region IEnumerable<IExpressionEvaluator> Members

		public IEnumerator<IExpressionEvaluator> GetEnumerator()
		{
			return _evaluators.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}
