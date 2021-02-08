using Microsoft.Extensions.Options;

namespace Isap.CommonCore.Cryptography
{
	public interface ICommonCryptoProvider
	{
		byte[] EncryptBytes(byte[] value);
		string EncryptString(string value);
		byte[] DecryptBytes(byte[] value);
		string DecryptString(string value);
	}

	public class CommonCryptoProvider: ICommonCryptoProvider
	{
		private readonly CommonCryptoProviderOptions _options;

		public CommonCryptoProvider(IOptions<CommonCryptoProviderOptions> options)
		{
			_options = options.Value;
		}

		public byte[] EncryptBytes(byte[] value)
		{
			return value.DesEncrypt(_options.Key, _options.IV);
		}

		public string EncryptString(string value)
		{
			return value.DesEncrypt(_options.Key, _options.IV);
		}

		public byte[] DecryptBytes(byte[] value)
		{
			return value.DesDecrypt(_options.Key, _options.IV);
		}

		public string DecryptString(string value)
		{
			return value.DesDecrypt(_options.Key, _options.IV);
		}
	}
}
