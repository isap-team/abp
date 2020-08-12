using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Isap.CommonCore.Utils
{
	public class DomainMatchResult<T>
	{
		public DomainMatchResult(List<T> values)
		{
			Values = values;
			Matches = new Dictionary<string, string>();
		}

		public List<T> Values { get; }
		public Dictionary<string, string> Matches { get; }
	}

	public interface IDomainSegmentMatcher
	{
		bool IsMatch(string segment, out Tuple<string, string>[] matchedValues);
		bool IsMatch(string segment);
	}

	public interface IDomainSegmentMatcher<T>: IDomainSegmentMatcher
	{
		IEnumerable<DomainMatchResult<T>> Match(Queue<string> segments);
		void Register(Queue<string> segments, T value);
	}

	public abstract class DomainSegmentMatcherBase<T>: IDomainSegmentMatcher<T>
	{
		public abstract bool IsMatch(string segment, out Tuple<string, string>[] matchedValues);

		public virtual bool IsMatch(string segment)
		{
			return IsMatch(segment, out _);
		}

		public abstract IEnumerable<DomainMatchResult<T>> Match(Queue<string> segments);

		public abstract void Register(Queue<string> segments, T value);
	}

	public class DomainSegmentMatcherCollection<T>: DomainSegmentMatcherBase<T>
	{
		private static readonly Regex __macroRegex = new Regex(@"\{(\d+)\}", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

		public List<IDomainSegmentMatcher<T>> Matchers { get; } = new List<IDomainSegmentMatcher<T>>();

		public List<T> Values { get; } = new List<T>();

		public override bool IsMatch(string segment, out Tuple<string, string>[] matchedValues)
		{
			foreach (IDomainSegmentMatcher<T> matcher in Matchers)
			{
				if (IsMatch(segment, out matchedValues))
					return true;
			}

			matchedValues = null;
			return false;
		}

		public override IEnumerable<DomainMatchResult<T>> Match(Queue<string> segments)
		{
			if (segments.Count == 0 || Matchers.Count == 0)
			{
				yield return new DomainMatchResult<T>(Values);
				yield break;
			}

			string segment = segments.Dequeue();
			foreach (IDomainSegmentMatcher<T> matcher in Matchers)
			{
				if (matcher.IsMatch(segment, out var matches))
				{
					foreach (DomainMatchResult<T> matchResult in matcher.Match(new Queue<string>(segments)))
					{
						if (matches != null)
							foreach (Tuple<string, string> match in matches)
								matchResult.Matches.Add(match.Item1, match.Item2);

						yield return matchResult;
					}
				}
			}
		}

		public override void Register(Queue<string> segments, T value)
		{
			if (segments.Count == 0)
			{
				Values.Add(value);
				return;
			}

			string segment = segments.Dequeue();
			(bool hasMacro, bool _, StringBuilder sb) = __macroRegex.Split(segment)
				.Aggregate((hasMacro: false, isMacro: false, sb: new StringBuilder()), (tuple, item) =>
					{
						tuple.sb.Append(tuple.isMacro ? $"(?<i{item}>.+)" : item);
						return (hasMacro: tuple.hasMacro || tuple.isMacro, isMacro: !tuple.isMacro, sb: tuple.sb);
					});
			string expr = sb.ToString();

			List<IDomainSegmentMatcher<T>> matchers;
			Func<string, IDomainSegmentMatcher<T>> ctor;
			if (hasMacro)
			{
				matchers = Matchers
					.OfType<RegexDomainSegmentMatcher<T>>()
					.Where(m => m.Expression == expr)
					.Cast<IDomainSegmentMatcher<T>>()
					.ToList();
				ctor = e => new RegexDomainSegmentMatcher<T>(e);
			}
			else
			{
				matchers = Matchers
					.Where(m => m.IsMatch(expr))
					.ToList();
				ctor = e => new EqualityDomainSegmentMatcher<T>(e);
			}

			if (matchers.Count > 0)
			{
				matchers.ForEach(m => m.Register(new Queue<string>(segments), value));
			}
			else
			{
				IDomainSegmentMatcher<T> matcher = ctor(expr);
				matcher.Register(segments, value);
				Matchers.Add(matcher);
			}
		}
	}

	public class RegexDomainSegmentMatcher<T>: DomainSegmentMatcherBase<T>
	{
		private readonly Regex _regex;

		public RegexDomainSegmentMatcher(string expression)
		{
			Expression = expression;
			_regex = new Regex(expression, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
		}

		public IDomainSegmentMatcher<T> Matchers { get; } = new DomainSegmentMatcherCollection<T>();

		public string Expression { get; }

		public override bool IsMatch(string segment, out Tuple<string, string>[] matchedValues)
		{
			Match match = _regex.Match(segment);
			if (match.Success)
			{
				matchedValues = _regex.GetGroupNames()
					.Where(name => name.StartsWith("i"))
					.Select(name => Tuple.Create(name, match.Groups[name].Value))
					.ToArray();
			}
			else
				matchedValues = null;

			return match.Success;
		}

		public override IEnumerable<DomainMatchResult<T>> Match(Queue<string> segments)
		{
			return Matchers.Match(segments);
		}

		public override void Register(Queue<string> segments, T value)
		{
			Matchers.Register(segments, value);
		}
	}

	public class EqualityDomainSegmentMatcher<T>: DomainSegmentMatcherBase<T>
	{
		public EqualityDomainSegmentMatcher(string expectedSegment)
		{
			ExpectedSegment = expectedSegment;
		}

		public IDomainSegmentMatcher<T> Matchers { get; } = new DomainSegmentMatcherCollection<T>();

		public string ExpectedSegment { get; }

		public override bool IsMatch(string segment, out Tuple<string, string>[] matchedValues)
		{
			matchedValues = null;
			return string.Equals(ExpectedSegment, segment, StringComparison.OrdinalIgnoreCase);
		}

		public override IEnumerable<DomainMatchResult<T>> Match(Queue<string> segments)
		{
			return Matchers.Match(segments);
		}

		public override void Register(Queue<string> segments, T value)
		{
			Matchers.Register(segments, value);
		}
	}

	public static class DomainSegmentMatchers
	{
		public static IDomainSegmentMatcher<T> Create<T>()
		{
			return new DomainSegmentMatcherCollection<T>();
		}

		public static IEnumerable<DomainMatchResult<T>> Match<T>(this IDomainSegmentMatcher<T> matcher, string domainName)
		{
			return matcher.Match(new Queue<string>(domainName.Split('.').Reverse()));
		}

		public static void Register<T>(this IDomainSegmentMatcher<T> matcher, string domainName, T value)
		{
			matcher.Register(new Queue<string>(domainName.Split('.').Reverse()), value);
		}
	}
}
