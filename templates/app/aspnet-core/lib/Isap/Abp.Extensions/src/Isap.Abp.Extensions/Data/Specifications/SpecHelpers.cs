using System;
using System.Collections.Generic;
using System.Text.Json;
using Isap.Abp.Extensions.Data.Specifications.FilterSpecs.Concrete;
using Isap.Abp.Extensions.Data.Specifications.OrderSpecs;
using Isap.CommonCore.Extensions;
using Isap.CommonCore.Services;

namespace Isap.Abp.Extensions.Data.Specifications
{
	public static class SpecHelpers
	{
		public static SpecificationParameters Create<TParams>(Guid specId, TParams parameters)
		{
			string json = JsonSerializer.Serialize(parameters, JsonExtensions.DefaultJsonSerializerOptions);
			JsonDocument doc = JsonDocument.Parse(json);
			return new SpecificationParameters(specId, doc.RootElement);
		}

		public static SpecificationParameters CreateOrderBySpec(Guid specId, bool isDescending = false)
		{
			return Create(specId, new OrderSpecificationParameters(isDescending));
		}

		public static SpecificationParameters CreateDirectFilterSpec<TValue>(Guid specId, TValue value)
		{
			return Create(specId, new DirectSpecificationParameters<TValue>(value));
		}

		public static SpecificationParameters CreateDirectSetFilterSpec<TValue>(Guid specId, params TValue[] values)
		{
			return Create(specId, new DirectSetSpecificationParameters<TValue>(values));
		}

		public static SpecificationParameters CreateRangeFilterSpec<TValue>(Guid specId, TValue fromValue, TValue toValue)
		{
			return Create(specId, new RangeSpecificationParameters<TValue>(fromValue, toValue));
		}

		public static List<SpecificationParameters> Create(params SpecificationParameters[] parameters) => new List<SpecificationParameters>(parameters);

		public static List<SpecificationParameters> AddOrReplace(this List<SpecificationParameters> list, SpecificationParameters parameters)
		{
			int idx = list.IndexOf(p => p.SpecId == parameters.SpecId);
			if (idx >= 0)
				list[idx] = parameters;
			else
				list.Add(parameters);
			return list;
		}
	}
}
