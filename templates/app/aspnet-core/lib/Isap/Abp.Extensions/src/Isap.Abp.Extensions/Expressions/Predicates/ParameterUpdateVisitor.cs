using System.Collections.Generic;
using System.Linq.Expressions;

namespace Isap.Abp.Extensions.Expressions.Predicates
{
	public class ParameterUpdateVisitor: ExpressionVisitor
	{
		readonly Dictionary<ParameterExpression, ParameterExpression> _map;

		private ParameterUpdateVisitor(Dictionary<ParameterExpression, ParameterExpression> map)
		{
			_map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
		}

		protected override Expression VisitParameter(ParameterExpression p)
		{
			if (_map.TryGetValue(p, out ParameterExpression replacement))
			{
				p = replacement;
			}

			return base.VisitParameter(p);
		}

		public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
		{
			return new ParameterUpdateVisitor(map).Visit(exp);
		}

		public static Expression ReplaceParameter(ParameterExpression oldParameter, ParameterExpression newParameter, Expression exp)
		{
			return new ParameterUpdateVisitor(new Dictionary<ParameterExpression, ParameterExpression> { { oldParameter, newParameter } }).Visit(exp);
		}
	}
}
