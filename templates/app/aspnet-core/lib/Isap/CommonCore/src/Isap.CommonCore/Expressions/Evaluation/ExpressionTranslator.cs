using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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
		}

		public string MacroBegin { get; }
		public string MacroEnd { get; }

		public Regex MacroSplitRegex { get; }

		public Regex MacroDefRegex { get; }

		public string Translate(string input, IExpressionEvaluator evaluator)
		{
			if (input == null)
				return null;

			string result = input;
			do
			{
				input = result;
				result = TranslateNext(input, evaluator);
			} while (!string.Equals(input, result));

			return result;
		}

		public string Translate(string input, IEvaluateExpressionValueProvider valueProvider)
		{
			return Translate(input, new ExpressionEvaluatorCollection
				{
					new CommonExpressionEvaluator(),
					new ValueProviderExpressionEvaluator(valueProvider),
				});
		}

		public string Translate(string input, Dictionary<string, string> values)
		{
			return Translate(input, new DictionaryEvaluateExpressionValueProvider(values));
		}

		public string Translate(string input, Dictionary<string, object> values)
		{
			return Translate(input, new DictionaryEvaluateExpressionValueProvider(values));
		}

		private static string EscapeChars(string chars)
		{
			return chars.Select(ch => "\\" + ch).Aggregate(String.Empty, (seed, item) => seed + item);
		}

		private string TranslateNext(string input, IExpressionEvaluator evaluator)
		{
			var result = new StringBuilder();
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
					string value;
					result.Append(evaluator.TryEvaluate(expr, out value) ? value : s);
				}
				else
					result.Append(s);

				isMacro = !isMacro;
			}

			return result.ToString();
		}
	}
}
