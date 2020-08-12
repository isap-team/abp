using System;

namespace Isap.CommonCore.API
{
	public class AuthTokenItem
	{
		public AuthTokenItem(string authToken, DateTime expiresAt)
		{
			AuthToken = authToken;
			ExpiresAt = expiresAt;
		}

		public AuthTokenItem(string authToken)
			: this(authToken, DateTime.MaxValue)
		{
		}

		public string AuthToken { get; set; }
		public DateTime ExpiresAt { get; set; }
	}
}
