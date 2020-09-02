using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Isap.CommonCore.Validation;
using Isap.Converters.Extensions;
using Newtonsoft.Json.Linq;
using Volo.Abp.Reflection;
using Volo.Abp.Validation;

namespace Isap.Abp.Extensions.Validation
{
	public class IsapMethodInvocationValidator: MethodInvocationValidator
	{
		public IsapMethodInvocationValidator(IObjectValidator objectValidator)
			: base(objectValidator)
		{
		}

		public override void Validate(MethodInvocationValidationContext context)
		{
			base.Validate(context);

			foreach (object parameterValue in context.ParameterValues)
			{
				if (parameterValue is MulticastDelegate)
					continue;

				Normalize(parameterValue);
			}
		}

		protected virtual void Normalize(object normalizingObject)
		{
			NormalizeObjectRecursively(new List<object>(), normalizingObject, 1);
		}

		protected virtual void NormalizeObjectRecursively(List<object> processedObjects, object normalizingObject, int currentDepth)
		{
			if (processedObjects.Contains(normalizingObject))
				return;

			processedObjects.Add(normalizingObject);

			if (normalizingObject.IsDefaultValue())
				return;

			Type validatingObjectType = normalizingObject.GetType();

			//Do not recursively validate for primitive objects
			if (TypeHelper.IsPrimitiveExtended(validatingObjectType))
				return;

			if (typeof(JObject).IsAssignableFrom(validatingObjectType))
				return;

			if (normalizingObject is ICommonNormalize normalizableObject)
			{
				normalizableObject.Normalize();
			}

			if (normalizingObject is IEnumerable enumerable)
			{
				if (enumerable is IQueryable)
					return;
				foreach (object item in enumerable)
				{
					NormalizeObjectRecursively(processedObjects, item, currentDepth + 1);
				}
			}

			var properties = TypeDescriptor.GetProperties(normalizingObject).Cast<PropertyDescriptor>();
			foreach (var property in properties)
			{
				NormalizeObjectRecursively(processedObjects, property.GetValue(normalizingObject), currentDepth + 1);
			}
		}
	}
}
