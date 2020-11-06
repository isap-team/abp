using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Isap.Converters;
using Isap.Converters.Extensions;

namespace Isap.CommonCore.Expressions.Evaluation
{
	public class ExpressionTranslator: IExpressionTranslator
	{
		public ExpressionTranslator(string macroBegin, string macroEnd)
		{
			MacroBegin = macroBegin;
			MacroEnd = macroEnd;
			string macroSplitRegex = String.Format(@"({0}[^{1}]+{1})", EscapeChars(macroBegin), EscapeChars(macroEnd));
			MacroSplitRegex = new Regex(macroSplitRegex, RegexOptions.Compiled | RegexOptions.Singleline);
			string macroDefRegex = String.Format(@"{0}(?<expression>[^{1}]+){1}", EscapeChars(macroBegin), EscapeChars(macroEnd));
			MacroDefRegex = new Regex(macroDefRegex, RegexOptions.Compiled | RegexOptions.Singleline);
			Converter = ValueConverterProviders.Default.GetConverter();
		}

		public string MacroBegin { get; }
		public string MacroEnd { get; }

		public Regex MacroSplitRegex { get; }

		public Regex MacroDefRegex { get; }

		public IValueConverter Converter { get; }

		public object Translate(string input, IExpressionEvaluator evaluator)
		{
			if (input == null)
				return null;

			object result = input;
			do
			{
				if (result is string strResult)
					input = strResult;
				else
					break;
				result = TranslateNext(input, evaluator);
			} while (!Converter.AreEquals(input, result));

			return result;
		}

		public object Translate(string input, IEvaluateExpressionValueProvider valueProvider)
		{
			return Translate(input, new ExpressionEvaluatorCollection
				{
					new CommonExpressionEvaluator(),
					new ValueProviderExpressionEvaluator(valueProvider),
				});
		}

		public object Translate(string input, Dictionary<string, string> values)
		{
			return Translate(input, new DictionaryEvaluateExpressionValueProvider(values));
		}

		public object Translate(string input, Dictionary<string, object> values)
		{
			return Translate(input, new DictionaryEvaluateExpressionValueProvider(values));
		}

		private static string EscapeChars(string chars)
		{
			return chars.Select(ch => "\\" + ch).Aggregate(String.Empty, (seed, item) => seed + item);
		}

		private object TranslateNext(string input, IExpressionEvaluator evaluator)
		{
			var result = new ExpressionResultBuilder();
			string[] strings = MacroSplitRegex.Split(input);
			bool isMacro = false;
			foreach (string s in strings)
			{
				if (isMacro)
				{
					Match match = MacroDefRegex.Match(s);
					if (!match.Success)
						throw new InvalidOperationException();
					string expr = match.Groups["expression"].Value;
					result.Append(evaluator.TryEvaluate(expr, out object value) ? value : s);
				}
				else
					result.Append(s);

				isMacro = !isMacro;
			}

			return result.Build();
		}
	}

	public class ExpressionResultBuilder
	{
		private readonly List<object> _values = new List<object>();

		public void Append(object value)
		{
			_values.Add(value);
		}

		public object Build()
		{
			_values.RemoveAll(v => v is string s && string.IsNullOrEmpty(s));
			switch (_values.Count)
			{
				case 0:
					return null;
				case 1:
					return _values.First();
				default:
					return string.Join(string.Empty, _values.Select(i => i?.ToString()));
			}
		}
	}
}
