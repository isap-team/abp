using System;
using System.Collections;
using System.Linq.Expressions;
using Volo.Abp.Data;

// ReSharper disable InconsistentNaming

namespace Isap.Abp.Extensions.Expressions.Predicates
{
	public interface IPredicateBuilder
	{
		Expression<Func<T, bool>> True<T>();
		Expression<Func<T, bool>> False<T>();

		Expression<Func<T, bool>> ILike<T>(Expression<Func<T, string>> propertyExpression, object value);
		Expression<Func<T, bool>> ILike<T>(string propertyName, object value);
		Expression<Func<T, bool>> Like<T>(string propertyName, object value);
		Expression<Func<T, bool>> InCollection<T>(string propertyName, ICollection values);
		Expression<Func<T, bool>> Equal<T>(string propertyName, object value);
		Expression<Func<T, TValue>> CreateMemberExpression<T, TValue>(string path);
		Expression<Func<T, bool>> LessThanOrEqual<T>(string propertyName, object value);
		Expression<Func<T, bool>> GreaterThanOrEqual<T>(string propertyName, object value);
		Expression<Func<T, bool>> Between<T>(string propertyName, object fromValue, object toValue);
		Expression<Func<T, bool>> Between2<T>(object value, string fromPropertyName, string toPropertyName);
		Expression<Func<T, bool>> Between2<T, TValue>(TValue value, Expression<Func<T, TValue>> fromExpr, Expression<Func<T, TValue>> toExpr);
		Expression<Func<T, bool>> BuildFindExpression<T>(LambdaExpression expression, object findValue) where T: class;
		MemberExpression CreateMemberExpression(ParameterExpression parameter, string path);

		Expression<Func<T, bool>> ExtraProperty<T, TValue>(Expression<Func<T, ExtraPropertyDictionary>> extraPropertyExpression,
			string propertyName, TValue value);

		string EscapeSqlString(string value);
	}
}
