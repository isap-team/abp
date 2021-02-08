using System;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Isap.Abp.Extensions.Web
{
	public abstract class IsapPageModel<TLocalizationResource>: AbpPageModel
		where TLocalizationResource: ILocalizationResource
	{
		protected IsapPageModel()
		{
			LocalizationResourceType = typeof(TLocalizationResource);
		}

		protected async Task<IActionResult> SafeAsync(Func<Task<IActionResult>> action)
		{
			try
			{
				return await action();
			}
			catch (Exception exception)
			{
				Logger.LogException(exception, LogLevel.Warning);
				Alerts.Danger(exception.Message);
				return Page();
			}
		}
	}
}
