using System;
using System.Collections.Generic;

namespace Isap.Abp.Extensions.Data.Specifications
{
	public class SpecificationMetadataDto
	{
		public Guid SpecId { get; set; }
		public string Name { get; set; }
		public List<string> Types { get; set; }
		public List<string> Namespace { get; set; }
	}
}
