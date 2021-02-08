using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Localization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Security.Encryption;

namespace Isap.Abp.Extensions.Web.Pages.Configuration
{
	public class SecureStringModel: IsapPageModel<AbpExtensionsResource>
	{
		protected IStringEncryptionService StringEncryptionService => LazyServiceProvider.LazyGetRequiredService<IStringEncryptionService>();

		[Required]
		[BindProperty]
		public string Text { get; set; }

		public string Result { get; set; }

		public void OnGet()
		{
		}

		public async Task<IActionResult> OnPostAsync(string action)
		{
			return await SafeAsync(async () =>
				{
					await Task.Yield();
					switch (action)
					{
						case "Encrypt":
							Result = StringEncryptionService.Encrypt(Text);
							break;
						case "Decrypt":
							Result = StringEncryptionService.Decrypt(Text);
							break;
						default:
							throw new NotSupportedException();
					}
					return Page();
				});
		}
	}
}
