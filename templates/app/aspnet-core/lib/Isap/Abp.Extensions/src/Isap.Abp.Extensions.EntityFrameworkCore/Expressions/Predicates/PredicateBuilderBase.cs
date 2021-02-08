using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Isap.Converters;
using Isap.Converters.Extensions;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;

namespace Isap.Abp.Extensions.Expressions.Predicates
{
	public abstract class PredicateBuilderBase: IPredicateBuilder
	{
		private static readonly MethodInfo _genericMethodContains = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
			.Where(m => m.Name == nameof(Enumerable.Contains))
			.Where(m => m.IsGenericMethodDefinition)
			.Select(m => Tuple.Create(m, m.GetGenericArguments(), m.GetParameters()))
			.Where(tuple => tuple.Item2.Length == 1 && tuple.Item3.Length == 2)
			.Where(tuple => tuple.Item3[0].ParameterType.IsGenericType && tuple.Item3[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
			.Select(tuple => tuple.Item1)
			.Single();

		public virtual Expression<Func<T, bool>> True<T>()
		{
			return f => true;
		}

		public virtual Expression<Func<T, bool>> False<T>()
		{
			return f => false;
		}

		public abstract Expression<Func<T, bool>> ILike<T>(Expression<Func<T, string>> propertyExpression, object value);

		public abstract Expression<Func<T, bool>> ILike<T>(string propertyName, object value);

		public virtual Expression<Func<T, bool>> Like<T>(string propertyName, object value)
		{
			ParameterExpression parameter = Expression.Parameter(typeof(T), "entry");
			MemberExpression memberExpression = CreateMemberExpression(parameter, propertyName);

			value = FixValueForProperty(memberExpression.Type, value);

			ConstantExpression constantExpression = Expression.Constant(value, memberExpression.Type);

			MethodInfo method =
				typeof(DbFunctionsExtensions).GetMethod(nameof(DbFunctionsExtensions.Like), new[] { typeof(DbFunctions), typeof(string), typeof(string) });

			Debug.Assert(method != null);

			return Expression.Lambda<Func<T, bool>>(Expression.Call(null, method, Expression.Constant(EF.Functions), memberExpression, constantExpression),
				parameter);
		}

		public virtual Expression<Func<T, bool>> InCollection<T>(string propertyName, ICollection values)
		{
			ParameterExpression parameter = Expression.Parameter(typeof(T), "entry");
			MemberExpression memberExpression = CreateMemberExpression(parameter, propertyName);

			object fixedValues = FixValueForProperty(typeof(List<>).MakeGenericType(memberExpression.Type), values);
			MethodInfo method = _genericMethodContains.MakeGenericMethod(memberExpression.Type);

			return Expression.Lambda<Func<T, bool>>(Expression.Call(null, method, Expression.Constant(fixedValues), memberExpression), parameter);
		}

		public virtual Expression<Func<T, bool>> Equal<T>(string propertyName, object value)
		{
			ParameterExpression parameter = Expression.Parameter(typeof(T), "entry");
			MemberExpression left = CreateMemberExpression(parameter, propertyName);

			value = FixValueForProperty(left.Type, value);

			ConstantExpression right = Expression.Constant(value, left.Type);
			return Expression.Lambda<Func<T, bool>>(Expression.Equal(left, right), parameter);
		}

		public virtual Expression<Func<T, TValue>> CreateMemberExpression<T, TValue>(string path)
		{
			ParameterExpression parameter = Expression.Parameter(typeof(T), "entry");
			MemberExpression expression = CreateMemberExpression(parameter, path);
			return Expression.Lambda<Func<T, TValue>>(expression, parameter);
		}

		public virtual Expression<Func<T, bool>> LessThanOrEqual<T>(string propertyName, object value)
		{
			ParameterExpression parameter = Expression.Parameter(typeof(T), "entry");
			MemberExpression left = CreateMemberExpression(parameter, propertyName);

			value = FixValueForProperty(left.Type, value);

			ConstantExpression right = Expression.Constant(value, left.Type);
			return Expression.Lambda<Func<T, bool>>(Expression.LessThanOrEqual(left, right), parameter);
		}

		public virtual Expression<Func<T, bool>> GreaterThanOrEqual<T>(string propertyName, object value)
		{
			ParameterExpression parameter = Expression.Parameter(typeof(T), "entry");
			MemberExpression left = CreateMemberExpression(parameter, propertyName);

			value = FixValueForProperty(left.Type, value);

			ConstantExpression right = Expression.Constant(value, left.Type);
			return Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(left, right), parameter);
		}

		public virtual Expression<Func<T, bool>> Between<T>(string propertyName, object fromValue, object toValue)
		{
			ParameterExpression parameter = Expression.Parameter(typeof(T), "entry");
			MemberExpression memberExpression = CreateMemberExpression(parameter, propertyName);

			fromValue = FixValueForProperty(memberExpression.Type, fromValue);
			toValue = FixValueForProperty(memberExpression.Type, toValue);

			ConstantExpression fromValueExpr = Expression.Constant(fromValue, memberExpression.Type);
			ConstantExpression toValueExpr = Expression.Constant(toValue, memberExpression.Type);

			Expression expression1 = Expression.GreaterThanOrEqual(memberExpression, fromValueExpr);
			Expression expression2 = Expression.LessThanOrEqual(memberExpression, toValueExpr);

			return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expression1, expression2), parameter);
		}

		public virtual Expression<Func<T, bool>> Between2<T>(object value, string fromPropertyName, string toPropertyName)
		{
			ParameterExpression parameter = Expression.Parameter(typeof(T), "entry");

			MemberExpression fromMemberExpr = CreateMemberExpression(parameter, fromPropertyName);

			MemberExpression toMemberExpr = CreateMemberExpression(parameter, toPropertyName);

			Debug.Assert(fromMemberExpr.Type == toMemberExpr.Type);

			value = FixValueForProperty(fromMemberExpr.Type, value);

			ConstantExpression valueExpr = Expression.Constant(value, fromMemberExpr.Type);

			Expression expression1 = Expression.LessThanOrEqual(fromMemberExpr, valueExpr);
			Expression expression2 = Expression.GreaterThanOrEqual(toMemberExpr, valueExpr);

			return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expression1, expression2), parameter);
		}

		public virtual Expression<Func<T, bool>> Between2<T, TValue>(TValue value, Expression<Func<T, TValue>> fromExpr, Expression<Func<T, TValue>> toExpr)
		{
			ParameterExpression parameter = Expression.Parameter(typeof(T), "entry");

			ConstantExpression valueExpr = Expression.Constant(value, typeof(TValue));

			var thisFromExpr = (Expression<Func<T, TValue>>) ParameterUpdateVisitor.ReplaceParameter(fromExpr.Parameters.First(), parameter, fromExpr);
			var thisToExpr = (Expression<Func<T, TValue>>) ParameterUpdateVisitor.ReplaceParameter(toExpr.Parameters.First(), parameter, toExpr);

			Debug.Assert(thisFromExpr != null);
			Debug.Assert(thisToExpr != null);

			Expression expression1 = Expression.LessThanOrEqual(thisFromExpr.Body, valueExpr);
			Expression expression2 = Expression.GreaterThanOrEqual(thisToExpr.Body, valueExpr);

			return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expression1, expression2), parameter);
		}

		public virtual Expression<Func<T, bool>> BuildFindExpression<T>(LambdaExpression expression, object findValue) where T: class
		{
			Type valueType = expression.Body.Type;

			findValue = FixValueForProperty(valueType, findValue);

			ConstantExpression right = Expression.Constant(findValue, valueType);
			return Expression.Lambda<Func<T, bool>>(Expression.Equal(expression.Body, right), expression.Parameters);
		}

		public virtual MemberExpression CreateMemberExpression(ParameterExpression parameter, string path)
		{
			string[] propertyNames = path.Split('.');
			return (MemberExpression) propertyNames.Aggregate((Expression) parameter, Expression.PropertyOrField);
		}

		public abstract Expression<Func<T, bool>> ExtraProperty<T, TValue>(Expression<Func<T, ExtraPropertyDictionary>> extraPropertyExpression,
			string propertyName, TValue value);

		public virtual string EscapeSqlString(string value)
		{
			return value;
		}

		protected virtual object FixValueForProperty(Type valueType, object value)
		{
			ConvertAttempt attempt = ValueConverterProviders.Default.GetConverter().TryConvertTo(valueType, value);
			value = attempt.IsSuccess ? attempt.Result : attempt.ResultType.GetDefaultValue();
			return value;
		}
	}
}
