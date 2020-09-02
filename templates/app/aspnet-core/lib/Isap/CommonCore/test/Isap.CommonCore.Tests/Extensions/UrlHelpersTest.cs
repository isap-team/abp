using System;
using Isap.CommonCore.Extensions;
using Shouldly;
using Xunit;

// ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local

namespace Isap.CommonCore.Tests.Extensions
{
	public class UrlHelpersTest
	{
		public class GetInput
		{
			public GetInput(int[] idList)
			{
				IdList = idList;
			}

			public int[] IdList { get; set; }
		}

		[Theory]
		[InlineData("ws://wapi.local:5000/ws", "tenantId", 2, "ws://wapi.local:5000/ws?tenantId=2")]
		[InlineData("ws://wapi.local:5000/ws/", "tenantId", 3, "ws://wapi.local:5000/ws/?tenantId=3")]
		[InlineData("ws://wapi.local:5000/ws/?tenantId=2", "tenantId", 3, "ws://wapi.local:5000/ws/?tenantId=3")]
		[InlineData("http://wapi.local:5000", "tenantId", 3, "http://wapi.local:5000/?tenantId=3")]
		public void SetQueryStringTest(string url, string paramName, object paramValue, string expectedUrl)
		{
			InternalSetQueryStringTest(new Uri(url), paramName, paramValue, expectedUrl);
		}

		private void InternalSetQueryStringTest(Uri url, string paramName, object paramValue, string expectedUrl)
		{
			string actualUrl = url.SetQueryString(paramName, paramValue).ToString();
			Assert.Equal(expectedUrl, actualUrl);
		}

		[Fact]
		public void SetQueryStringTest2()
		{
			Uri actual = new Uri("http://localhost").SetQueryString(new GetInput(new[] { 1, 2, 4 }), false);
			actual.ShouldNotBeNull();
			actual.ToString().ShouldBe("http://localhost/?IdList=1&IdList=2&IdList=4");
		}

		[Theory]
		[InlineData(null, null, null)]
		[InlineData("http://wapi.local:5000", "http://wapi.local:5000/", "/")]
		[InlineData("http://wapi.local:5000/", "http://wapi.local:5000/", "/")]
		[InlineData("http://wapi.local:5000/Segment0", "http://wapi.local:5000/", "Segment0")]
		[InlineData("http://wapi.local:5000/Segment0/", "http://wapi.local:5000/", "Segment0/")]
		[InlineData("http://wapi.local:5000/Segment0/Segment1", "http://wapi.local:5000/Segment0/", "Segment1")]
		[InlineData("http://wapi.local:5000/Segment0/Segment1/", "http://wapi.local:5000/Segment0/", "Segment1/")]
		public void RemoveLastSegmentTest(string url, string expectedUrl, string expectedLastSegment)
		{
			InternalRemoveLastSegmentTest(url == null ? null : new Uri(url), expectedUrl, expectedLastSegment);
		}

		private void InternalRemoveLastSegmentTest(Uri url, string expectedUrl, string expectedLastSegment)
		{
			var (uri, lastSegment) = url.RemoveLastSegment();

			uri?.AbsoluteUri.ShouldBe(expectedUrl);
			lastSegment.ShouldBe(expectedLastSegment);
		}
	}
}
