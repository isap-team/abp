using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Isap.Abp.BackgroundJobs.EntityFrameworkCore
{
	public class JsonObjectWrapperValueConverter: ValueConverter<object, string>
	{
		public JsonObjectWrapperValueConverter([CanBeNull] ConverterMappingHints mappingHints = null)
			: base(value => ThisConvertToProviderExpression(value), value => ThisConvertFromProviderExpression(value), mappingHints)
		{
		}

		private static string ThisConvertToProviderExpression(object value)
		{
			string typeName = value?.GetType().AssemblyQualifiedName;
			if (typeName == null)
				return null;
			var result = new JsonObjectWrapper { TypeName = typeName, Value = JToken.FromObject(value) };
			return JToken.FromObject(result).ToString(Formatting.None);
		}

		private static object ThisConvertFromProviderExpression(string value)
		{
			if (string.IsNullOrEmpty(value))
				return null;
			var jobResult = JToken.Parse(value).ToObject<JsonObjectWrapper>();
			Type type = Type.GetType(jobResult.TypeName);
			return jobResult.Value?.ToObject(type);
		}
	}
}
