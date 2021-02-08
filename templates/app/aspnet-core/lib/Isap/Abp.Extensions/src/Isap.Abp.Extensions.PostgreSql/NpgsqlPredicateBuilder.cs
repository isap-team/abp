using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Isap.Abp.Extensions.Expressions.Predicates;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore.ValueConverters;

namespace Isap.Abp.Extensions.PostgreSql
{
	public class NpgsqlPredicateBuilder: PredicateBuilderBase
	{
		public override Expression<Func<T, bool>> ILike<T>(Expression<Func<T, string>> propertyExpression, object value)
		{
			ParameterExpression parameter = Expression.Parameter(typeof(T), "entry");

			propertyExpression =
				(Expression<Func<T, string>>) ParameterUpdateVisitor.ReplaceParameter(propertyExpression.Parameters.First(), parameter, propertyExpression);
			var memberExpression = (MemberExpression) propertyExpression.Body;

			value = FixValueForProperty(memberExpression.Type, value);

			ConstantExpression constantExpression = Expression.Constant(value, memberExpression.Type);

			MethodInfo method =
				typeof(NpgsqlDbFunctionsExtensions).GetMethod(nameof(NpgsqlDbFunctionsExtensions.ILike),
					new[] { typeof(DbFunctions), typeof(string), typeof(string) });

			Debug.Assert(method != null);

			return Expression.Lambda<Func<T, bool>>(Expression.Call(null, method, Expression.Constant(EF.Functions), memberExpression, constantExpression),
				parameter);
		}

		public override Expression<Func<T, bool>> ILike<T>(string propertyName, object value)
		{
			ParameterExpression parameter = Expression.Parameter(typeof(T), "entry");
			MemberExpression memberExpression = CreateMemberExpression(parameter, propertyName);

			value = FixValueForProperty(memberExpression.Type, value);

			ConstantExpression constantExpression = Expression.Constant(value, memberExpression.Type);

			MethodInfo method =
				typeof(NpgsqlDbFunctionsExtensions).GetMethod(nameof(NpgsqlDbFunctionsExtensions.ILike),
					new[] { typeof(DbFunctions), typeof(string), typeof(string) });

			Debug.Assert(method != null);

			return Expression.Lambda<Func<T, bool>>(Expression.Call(null, method, Expression.Constant(EF.Functions), memberExpression, constantExpression),
				parameter);
		}

		public override Expression<Func<T, bool>> ExtraProperty<T, TValue>(Expression<Func<T, ExtraPropertyDictionary>> extraPropertyExpression, string propertyName, TValue value)
		{
			var converter = new AbpJsonValueConverter<TValue>();
			object strValue = converter.ConvertToProvider(value);
			return e => EF.Functions.JsonContains(extraPropertyExpression, @$"{{""{propertyName}"": {strValue}}}");
		}
	}
}
