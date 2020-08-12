using System;
using Microsoft.Extensions.DependencyInjection;
using Isap.CommonCore.Validation;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Validation;

namespace Isap.Abp.Extensions.Validation
{
	public class CustomValidationContributor: IObjectValidationContributor, ITransientDependency
	{
		private readonly IServiceProvider _serviceProvider;

		public CustomValidationContributor(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public void AddErrors(ObjectValidationContext context)
		{
			if (context.ValidatingObject is ICommonValidation commonValidation)
			{
				commonValidation.AddErrors(new CommonValidationContext(context.Errors, serviceType => _serviceProvider.GetRequiredService(serviceType)));
			}
		}
	}
}
