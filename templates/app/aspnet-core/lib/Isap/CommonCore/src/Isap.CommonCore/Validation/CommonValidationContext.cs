using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Isap.CommonCore.Validation
{
	public class CommonValidationContext
	{
		private readonly Func<Type, object> _getRequiredService;

		public CommonValidationContext(List<ValidationResult> errors, Func<Type, object> getRequiredService)
		{
			_getRequiredService = getRequiredService;
			Errors = errors;
		}

		public List<ValidationResult> Errors { get; }

		public object GetRequiredService(Type serviceType) => _getRequiredService(serviceType);
		public TService GetRequiredService<TService>() => (TService) _getRequiredService(typeof(TService));
	}
}
