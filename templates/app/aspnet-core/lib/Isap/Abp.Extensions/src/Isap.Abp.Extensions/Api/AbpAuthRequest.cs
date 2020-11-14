using System.ComponentModel.DataAnnotations;

namespace Isap.Abp.Extensions.Api
{
	public class AbpAuthRequest
	{
		public const int MaxEmailAddressLength = 256;
		public const int MaxPlainPasswordLength = 32;

		[Required]
		[StringLength(MaxEmailAddressLength)]
		public string UserNameOrEmailAddress { get; set; }

		[Required]
		[StringLength(MaxPlainPasswordLength)]
		public string Password { get; set; }

		public bool RememberClient { get; set; }

		public int? TenantId { get; set; }
	}
}
