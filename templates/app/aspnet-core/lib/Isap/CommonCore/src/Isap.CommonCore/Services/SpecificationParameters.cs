using System;
using System.Text.Json;
using Isap.CommonCore.Extensions;

namespace Isap.CommonCore.Services
{
	[Serializable]
	public class SpecificationParameters: ISpecificationParameters, ISpecificationParametersProvider
	{
		public SpecificationParameters(Guid specId, JsonElement parameters)
		{
			SpecId = specId;
			Parameters = parameters;
		}

		public SpecificationParameters()
		{
		}

		public Guid SpecId { get; set; }

		public JsonElement Parameters { get; set; }
		ISpecificationParametersProvider ISpecificationParameters.Parameters => this;

		TParams ISpecificationParametersProvider.GetParameters<TParams>() => GetParams<TParams>();

		protected virtual T GetParams<T>()
		{
			return Parameters.ToObject<T>();
		}
	}

	public class SpecificationParameters<TParams>: SpecificationParameters
	{
		public SpecificationParameters(Guid specId, TParams parameters)
		{
			SpecId = specId;
			Parameters = parameters;
		}

		public new TParams Parameters { get; set; }

		protected override T GetParams<T>()
		{
			if (!typeof(T).IsAssignableFrom(typeof(TParams)))
				throw new InvalidOperationException();
			return (T) (object) Parameters;
		}
	}
}
