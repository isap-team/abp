using Isap.CommonCore.Cryptography;
using Isap.CommonCore.Utils;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

// ReSharper disable InconsistentNaming

namespace Isap.CommonCore.Tests.Cryptography
{
	public class CommonCryptoProviderTest
	{
		public const string DefaultKey = "CA11E0C1D0FDA936";
		public const string DefaultIV = "8063F287DD3A8659";

		private static readonly IOptions<CommonCryptoProviderOptions> _options = new OptionsWrapper<CommonCryptoProviderOptions>(
			new CommonCryptoProviderOptions
				{
					Key = HexUtility.StringToByteArray(DefaultKey),
					IV = HexUtility.StringToByteArray(DefaultIV),
				});

		[Theory]
		[InlineData("123qwe", "mfxfz7vNvZQ=")]
		public void EncryptTest(string value, string expectedValue)
		{
			ICommonCryptoProvider target = new CommonCryptoProvider(_options);
			string actualResult = target.EncryptString(value);
			actualResult.ShouldBe(expectedValue);
		}

		[Theory]
		[InlineData("mfxfz7vNvZQ=", "123qwe")]
		public void DecryptTest(string value, string expectedValue)
		{
			ICommonCryptoProvider target = new CommonCryptoProvider(_options);
			string actualResult = target.DecryptString(value);
			actualResult.ShouldBe(expectedValue);
		}
	}
}
