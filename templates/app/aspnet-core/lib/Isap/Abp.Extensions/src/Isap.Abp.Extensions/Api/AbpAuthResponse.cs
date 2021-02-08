namespace Isap.Abp.Extensions.Api
{
	public interface IAbpAuthResponse
	{
		string AccessToken { get; }
		string EncryptedAccessToken { get; }
		int ExpireInSeconds { get; }
		bool WaitingForActivation { get; }
		long UserId { get; }
		int NodeKey { get; }
	}

	public class AbpAuthResponse: IAbpAuthResponse
	{
		public string AccessToken { get; set; }
		public string EncryptedAccessToken { get; set; }
		public int ExpireInSeconds { get; set; }
		public bool WaitingForActivation { get; set; }
		public long UserId { get; set; }
		public int NodeKey { get; set; }
	}
}
