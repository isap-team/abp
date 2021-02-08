using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Domain;
using Isap.Abp.Extensions.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.Validation;

namespace Isap.Abp.Extensions.Web
{
	public abstract class IsapPageModel<TLocalizationResource>: AbpPageModel, ISupportsLazyServices
		where TLocalizationResource: ILocalizationResource
	{
		protected IsapPageModel()
		{
			LocalizationResourceType = typeof(TLocalizationResource);
		}

		object ISupportsLazyServices.ServiceProviderLock => ServiceProviderLock;
		ConcurrentDictionary<Type, object> ISupportsLazyServices.ServiceReferenceMap { get; } = new ConcurrentDictionary<Type, object>();

		protected TService LazyGetRequiredService<TService>()
		{
			return SupportsLazyServicesExtensions.LazyGetRequiredService<TService>(this);
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
