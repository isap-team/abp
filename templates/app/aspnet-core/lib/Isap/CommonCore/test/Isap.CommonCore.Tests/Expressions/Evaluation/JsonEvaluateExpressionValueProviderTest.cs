using System;
using Isap.CommonCore.Expressions.Evaluation;
using Isap.Converters;
using Newtonsoft.Json.Linq;
using Shouldly;
using Xunit;

namespace Isap.CommonCore.Tests.Expressions.Evaluation
{
	public class JsonEvaluateExpressionValueProviderTest
	{
		internal class SampleInnerObject
		{
			public string Value { get; set; }
		}

		internal class SampleOuterObject
		{
			public string Name { get; set; }
			public DateTime Date { get; set; }
			public SampleInnerObject Inner { get; set; }
		}

		[Theory]
		[InlineData("Name", typeof(string), "Object1")]
		[InlineData("Inner.Value", typeof(string), "Value1")]
		[InlineData("Date", typeof(DateTime), "15.03.2020 19:24:35")]
		[InlineData("Date:yyyy.MM", typeof(string), "2020.03")]
		public void TryGetValueTest(string id, Type expectedResultType, string expectedResult)
		{
			var converter = ValueConverterProviders.Default.GetConverter();
			object convertedExpectedResult = converter.ConvertTo(expectedResultType, expectedResult);

			var input = new SampleOuterObject
				{
					Name = "Object1",
					Date = new DateTime(2020, 3, 15, 19, 24, 35),
					Inner = new SampleInnerObject
						{
							Value = "Value1",
						},
				};

			var target = new JsonEvaluateExpressionValueProvider(JObject.FromObject(input));

			target.TryGetValue(id, out object actualResult).ShouldBeTrue();
			actualResult.ShouldBe(convertedExpectedResult);
		}
	}
}
