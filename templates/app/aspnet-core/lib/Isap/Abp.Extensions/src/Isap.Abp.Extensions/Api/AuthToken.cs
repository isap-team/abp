using System;

namespace Isap.Abp.Extensions.Api
{
	public class AuthToken
	{
		public AuthToken(string data, string encryptedData, DateTime expireAt)
		{
			Data = data;
			EncryptedData = encryptedData;
			ExpireAt = expireAt;
		}

		/// <summary>
		///     Авторизационные данные, подтверждающие доступ пользователя к ресурсам сервера.
		/// </summary>
		public string Data { get; }

		/// <summary>
		///     Зашифрованные авторизационные данные, подтверждающие доступ пользователя к ресурсам сервера.
		/// </summary>
		public string EncryptedData { get; }

		/// <summary>
		///     Время, в течении которого авторизационные данные будут приниматься сервером.
		/// </summary>
		public DateTime ExpireAt { get; }
	}
}
