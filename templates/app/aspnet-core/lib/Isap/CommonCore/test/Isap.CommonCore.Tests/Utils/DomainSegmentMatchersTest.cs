using System.Collections.Generic;
using System.Linq;
using Isap.CommonCore.Utils;
using Shouldly;
using Xunit;

namespace Isap.CommonCore.Tests.Utils
{
	public class DomainSegmentMatchersTest
	{
		[Fact]
		public void RegisterTest()
		{
			IDomainSegmentMatcher<int> target = DomainSegmentMatchers.Create<int>();
			DomainSegmentMatcherCollection<int> actual = target.ShouldBeOfType<DomainSegmentMatcherCollection<int>>();
			actual.Matchers.ShouldNotBeNull();
			actual.Matchers.Count.ShouldBe(0);

			#region Зарегистрируем "{0}.amocrm.ru"

			target.Register("{0}.amocrm.ru", 1);

			actual = target.ShouldBeOfType<DomainSegmentMatcherCollection<int>>();

			actual.Values.ShouldNotBeNull();
			actual.Values.ShouldBeEmpty();
			actual.Matchers.ShouldNotBeNull();
			actual.Matchers.Count.ShouldBe(1);
			{
				EqualityDomainSegmentMatcher<int> actualMatcher = actual.Matchers[0].ShouldBeOfType<EqualityDomainSegmentMatcher<int>>();
				actualMatcher.ExpectedSegment.ShouldBe("ru");
				actual = actualMatcher.Matchers.ShouldBeOfType<DomainSegmentMatcherCollection<int>>();
			}

			actual.Values.ShouldNotBeNull();
			actual.Values.ShouldBeEmpty();
			actual.Matchers.ShouldNotBeNull();
			actual.Matchers.Count.ShouldBe(1);
			{
				EqualityDomainSegmentMatcher<int> actualMatcher = actual.Matchers[0].ShouldBeOfType<EqualityDomainSegmentMatcher<int>>();
				actualMatcher.ExpectedSegment.ShouldBe("amocrm");
				actual = actualMatcher.Matchers.ShouldBeOfType<DomainSegmentMatcherCollection<int>>();
			}

			actual.Values.ShouldNotBeNull();
			actual.Values.ShouldBeEmpty();
			actual.Matchers.ShouldNotBeNull();
			actual.Matchers.Count.ShouldBe(1);
			{
				RegexDomainSegmentMatcher<int> actualMatcher = actual.Matchers[0].ShouldBeOfType<RegexDomainSegmentMatcher<int>>();
				actualMatcher.Expression.ShouldBe("(?<i0>.+)");
				actual = actualMatcher.Matchers.ShouldBeOfType<DomainSegmentMatcherCollection<int>>();
			}

			actual.Matchers.ShouldNotBeNull();
			actual.Matchers.ShouldBeEmpty();
			actual.Values.ShouldNotBeNull();
			actual.Values.Count.ShouldBe(1);
			actual.Values[0].ShouldBe(1);

			#endregion

			#region Зарегистрируем "{0}.megaplan.ru"

			target.Register("{0}.megaplan.ru", 2);

			actual = target.ShouldBeOfType<DomainSegmentMatcherCollection<int>>();

			actual.Values.ShouldNotBeNull();
			actual.Values.ShouldBeEmpty();
			actual.Matchers.ShouldNotBeNull();
			actual.Matchers.Count.ShouldBe(1);
			{
				EqualityDomainSegmentMatcher<int> actualMatcher = actual.Matchers[0].ShouldBeOfType<EqualityDomainSegmentMatcher<int>>();
				actualMatcher.ExpectedSegment.ShouldBe("ru");
				actual = actualMatcher.Matchers.ShouldBeOfType<DomainSegmentMatcherCollection<int>>();
			}

			actual.Values.ShouldNotBeNull();
			actual.Values.ShouldBeEmpty();
			actual.Matchers.ShouldNotBeNull();
			actual.Matchers.Count.ShouldBe(2);
			{
				EqualityDomainSegmentMatcher<int> actualMatcher = actual.Matchers[0].ShouldBeOfType<EqualityDomainSegmentMatcher<int>>();
				actualMatcher.ExpectedSegment.ShouldBe("amocrm");
				actualMatcher = actual.Matchers[1].ShouldBeOfType<EqualityDomainSegmentMatcher<int>>();
				actualMatcher.ExpectedSegment.ShouldBe("megaplan");
				actual = actualMatcher.Matchers.ShouldBeOfType<DomainSegmentMatcherCollection<int>>();
			}

			actual.Values.ShouldNotBeNull();
			actual.Values.ShouldBeEmpty();
			actual.Matchers.ShouldNotBeNull();
			actual.Matchers.Count.ShouldBe(1);
			{
				RegexDomainSegmentMatcher<int> actualMatcher = actual.Matchers[0].ShouldBeOfType<RegexDomainSegmentMatcher<int>>();
				actualMatcher.Expression.ShouldBe("(?<i0>.+)");
				actual = actualMatcher.Matchers.ShouldBeOfType<DomainSegmentMatcherCollection<int>>();
			}

			actual.Matchers.ShouldNotBeNull();
			actual.Matchers.ShouldBeEmpty();
			actual.Values.ShouldNotBeNull();
			actual.Values.Count.ShouldBe(1);
			actual.Values[0].ShouldBe(2);

			#endregion

			#region Зарегистрируем "xxx.ru"

			target.Register("xxx.ru", 3);

			actual = target.ShouldBeOfType<DomainSegmentMatcherCollection<int>>();

			actual.Values.ShouldNotBeNull();
			actual.Values.ShouldBeEmpty();
			actual.Matchers.ShouldNotBeNull();
			actual.Matchers.Count.ShouldBe(1);
			{
				EqualityDomainSegmentMatcher<int> actualMatcher = actual.Matchers[0].ShouldBeOfType<EqualityDomainSegmentMatcher<int>>();
				actualMatcher.ExpectedSegment.ShouldBe("ru");
				actual = actualMatcher.Matchers.ShouldBeOfType<DomainSegmentMatcherCollection<int>>();
			}

			actual.Values.ShouldNotBeNull();
			actual.Values.ShouldBeEmpty();
			actual.Matchers.ShouldNotBeNull();
			actual.Matchers.Count.ShouldBe(3);
			{
				EqualityDomainSegmentMatcher<int> actualMatcher = actual.Matchers[0].ShouldBeOfType<EqualityDomainSegmentMatcher<int>>();
				actualMatcher.ExpectedSegment.ShouldBe("amocrm");
				actualMatcher = actual.Matchers[1].ShouldBeOfType<EqualityDomainSegmentMatcher<int>>();
				actualMatcher.ExpectedSegment.ShouldBe("megaplan");
				actualMatcher = actual.Matchers[2].ShouldBeOfType<EqualityDomainSegmentMatcher<int>>();
				actualMatcher.ExpectedSegment.ShouldBe("xxx");
				actual = actualMatcher.Matchers.ShouldBeOfType<DomainSegmentMatcherCollection<int>>();
			}

			actual.Matchers.ShouldNotBeNull();
			actual.Matchers.ShouldBeEmpty();
			actual.Values.ShouldNotBeNull();
			actual.Values.Count.ShouldBe(1);
			actual.Values[0].ShouldBe(3);

			#endregion
		}

		[Fact]
		public void MatchTest()
		{
			IDomainSegmentMatcher<int> target = DomainSegmentMatchers.Create<int>();
			target.Register("{0}.amocrm.ru", 1);
			target.Register("{0}.megaplan.ru", 2);
			target.Register("xxx.ru", 3);

			List<DomainMatchResult<int>> actual;

			#region Проверим поиск для зарегистрированного домена "dev.megaplan.ru"

			actual = target.Match("dev.megaplan.ru").ToList();

			actual.Count.ShouldBe(1);
			actual[0].Values.ShouldNotBeNull();
			actual[0].Values.Count.ShouldBe(1);
			actual[0].Values[0].ShouldBe(2);
			actual[0].Matches.ContainsKey("i0").ShouldBeTrue();
			actual[0].Matches["i0"].ShouldBe("dev");

			#endregion

			#region Проверим поиск для зарегистрированного домена "www.xxx.ru"

			actual = target.Match("www.xxx.ru").ToList();

			actual.Count.ShouldBe(1);
			actual[0].Values.ShouldNotBeNull();
			actual[0].Values.Count.ShouldBe(1);
			actual[0].Values[0].ShouldBe(3);
			actual[0].Matches.Count.ShouldBe(0);

			#endregion

			#region Проверим поиск для незарегистрированного домена "www.yyy.ru"

			actual = target.Match("www.yyy.ru").ToList();

			actual.ShouldBeEmpty();

			#endregion
		}
	}
}
