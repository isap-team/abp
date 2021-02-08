using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Isap.Abp.Extensions.DataFilters.Converters;
using Isap.Abp.Extensions.Expressions.Predicates;
using Isap.Converters;
using Newtonsoft.Json.Linq;

namespace Isap.Abp.Extensions.DataFilters
{
	public abstract class DataFilterStrategyBase<TEntity>: IDataFilterStrategy<TEntity>
	{
		private readonly Dictionary<string, object> _options;

		protected DataFilterStrategyBase(Dictionary<string, object> options)
		{
			_options = options;
		}

		public IValueConverter Converter { get; set; }
		public IPredicateBuilder PredicateBuilder { get; set; }
		public IDataFilterValueConverterFactory DataFilterValueConverterFactory { get; set; }
		public ICustomPredicateBuilderFactory CustomPredicateBuilderFactory { get; set; }

		public Expression<Func<TEntity, bool>> CreateFilterExpression(Dictionary<string, object> options)
		{
			options = _options.Replace(options);
			Expression<Func<TEntity, bool>> expression = CreateExpression(options);
			if (options.TryGetValue(nameof(DataFilterOptions.IsInverted), out object optionValue)
				&& Converter.TryConvertTo<bool>(optionValue).AsDefaultIfNotSuccess()
			)
			{
				expression = expression.Not();
			}

			return expression;
		}

		public virtual Expression<Func<TEntity, bool>> CreateFilterExpression(JToken options)
		{
			return CreateFilterExpression(DataFilterOptionsExtensions.Deserialize(options));
		}

		public Expression<Func<TEntity, bool>> CreateFilterExpression(string options)
		{
			return CreateFilterExpression(DataFilterOptionsExtensions.Deserialize(options));
		}

		public virtual Dictionary<string, object> CompleteOptions(Dictionary<string, object> options)
		{
			return options;
		}

		public virtual JToken CompleteOptions(JToken options)
		{
			return JToken.FromObject(CompleteOptions(DataFilterOptionsExtensions.Deserialize(options)));
		}

		Expression IDataFilterStrategy.CreateFilterExpression(Dictionary<string, object> options)
		{
			return CreateFilterExpression(options);
		}

		Expression IDataFilterStrategy.CreateFilterExpression(JToken options)
		{
			return CreateFilterExpression(options);
		}

		Expression IDataFilterStrategy.CreateFilterExpression(string options)
		{
			return CreateFilterExpression(options);
		}

		protected abstract Expression<Func<TEntity, bool>> CreateExpression(Dictionary<string, object> options);

		protected static bool IsString(object value, out string searchExpr)
		{
			searchExpr = null;
			switch (value)
			{
				case char ch:
					searchExpr = new string(ch, 1);
					break;
				case string str:
					searchExpr = str;
					break;
				default:
					return false;
			}

			searchExpr = searchExpr.Replace("'", "''"); // устранение возможности SQL-инъекции
			return true;
		}
	}
}
