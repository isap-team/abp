using System;
using System.Collections.Generic;

namespace Isap.CommonCore.Expressions.Evaluation
{
	public class CommonExpressionEvaluator: IExpressionEvaluator
	{
		private delegate object TranslateExpressionDelegate(string expression, string format);

		private static readonly Dictionary<string, TranslateExpressionDelegate> _stdMacroses =
			new Dictionary<string, TranslateExpressionDelegate>(StringComparer.OrdinalIgnoreCase)
				{
					{ "#dateTime", (expr, format) => DateTime.Now.ToString(format ?? "yyyy.MM.dd.HH.mm.ss") },
					{ "#date", (expr, format) => DateTime.Today.ToString(format ?? "yyyy.MM.dd") },
					{ "#currDir", (expr, format) => Environment.CurrentDirectory },
				};

		public bool TryEvaluate(string expression, out object result)
		{
			result = null;
			if (expression.StartsWith("#"))
			{
				int idx = expression.LastIndexOf(':');
				string format = idx < 0 ? null : expression.Substring(idx + 1);
				expression = idx < 0 ? expression : expression.Substring(0, idx);
				if (!_stdMacroses.TryGetValue(expression, out TranslateExpressionDelegate handler))
					return false;
				result = handler(expression, format);
				return true;
			}

			if (expression.StartsWith("%"))
			{
				if (!EnumTryParse(expression.Substring(1), out Environment.SpecialFolder folder))
					return false;
				result = Environment.GetFolderPath(folder);
				return true;
			}

			return false;
		}

		private static bool EnumTryParse<T>(string value, out T result)
		{
			try
			{
				result = (T) Enum.Parse(typeof(T), value, true);
				return true;
			}
			catch (ArgumentException)
			{
				result = default(T);
				return false;
			}
		}
	}
}
