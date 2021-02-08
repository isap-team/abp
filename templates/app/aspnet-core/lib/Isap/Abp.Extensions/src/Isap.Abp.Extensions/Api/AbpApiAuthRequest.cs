using System;
using System.ComponentModel.DataAnnotations;

namespace Isap.Abp.Extensions.Api
{
	public class AbpApiAuthRequest
	{
		public const int MaxEmailAddressLength = 256;
		public const int MaxApiKeyLength = 32;

		public Guid? UserUid { get; set; }

		[StringLength(MaxEmailAddressLength)]
		public string UserNameOrEmailAddress { get; set; }

		[Required]
		[StringLength(MaxApiKeyLength)]
		public string ApiKey { get; set; }

		public int? TenantId { get; set; }
	}
}
