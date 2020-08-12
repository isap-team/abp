using System;
using Newtonsoft.Json.Linq;

namespace Isap.CommonCore.Services
{
	[Serializable]
	public class SpecificationParameters
	{
		public SpecificationParameters(Guid specId, JToken parameters)
		{
			SpecId = specId;
			Parameters = parameters;
		}

		public SpecificationParameters()
		{
		}

		public Guid SpecId { get; set; }
		public JToken Parameters { get; set; }
	}
}
