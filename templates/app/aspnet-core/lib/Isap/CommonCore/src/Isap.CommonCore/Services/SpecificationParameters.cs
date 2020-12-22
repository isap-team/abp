using System;
using System.Text.Json;

namespace Isap.CommonCore.Services
{
	[Serializable]
	public class SpecificationParameters
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
	}
}
