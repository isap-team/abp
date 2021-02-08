using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Isap.CommonCore.Cryptography
{
	public static class CryptoExtensions
	{
		public static byte[] TransformWrite(this byte[] bytes, ICryptoTransform transform)
		{
			using (var inputStream = new MemoryStream(bytes))
			using (var outputStream = new MemoryStream())
			{
				using (var cryptoStream = new CryptoStream(outputStream, transform, CryptoStreamMode.Write))
					inputStream.CopyTo(cryptoStream);

				return outputStream.ToArray();
			}
		}

		public static byte[] TransformRead(this byte[] bytes, ICryptoTransform transform)
		{
			using (var inputStream = new MemoryStream(bytes))
			using (var outputStream = new MemoryStream())
			{
				using (var cryptoStream = new CryptoStream(inputStream, transform, CryptoStreamMode.Read))
					cryptoStream.CopyTo(outputStream);
				return outputStream.ToArray();
			}
		}

		public static byte[] DesEncrypt(this byte[] bytes, byte[] rgbKey, byte[] rgbIV)
		{
			DES des = new DESCryptoServiceProvider();
			return TransformWrite(bytes, des.CreateEncryptor(rgbKey, rgbIV));
		}

		public static string DesEncrypt(this string value, byte[] rgbKey, byte[] rgbIV)
		{
			byte[] result = DesEncrypt(Encoding.UTF8.GetBytes(value), rgbKey, rgbIV);
			return Convert.ToBase64String(result);
		}

		public static byte[] DesDecrypt(this byte[] bytes, byte[] rgbKey, byte[] rgbIV)
		{
			DES des = new DESCryptoServiceProvider();
			return TransformRead(bytes, des.CreateDecryptor(rgbKey, rgbIV));
		}

		public static string DesDecrypt(this string value, byte[] rgbKey, byte[] rgbIV)
		{
			byte[] result = DesDecrypt(Convert.FromBase64String(value), rgbKey, rgbIV);
			return Encoding.UTF8.GetString(result);
		}

		public static (byte[] rgbKey, byte[] rgbIV) DesGenerateKey()
		{
			DES des = new DESCryptoServiceProvider();
			return (des.Key, des.IV);
		}
	}
}
