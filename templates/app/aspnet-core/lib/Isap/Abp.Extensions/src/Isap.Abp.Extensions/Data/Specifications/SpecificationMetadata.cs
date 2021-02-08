using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Isap.Abp.Extensions.Data.Specifications
{
	public class SpecificationMetadata
	{
		public Guid SpecId { get; set; }
		public string Name { get; set; }
		public List<Type> Types { get; set; } = new List<Type>();
		public List<string> Namespace { get; set; } = new List<string>();

		public static SpecificationMetadata Create(Guid specId, MemberInfo member)
		{
			var info = new SpecificationMetadata
				{
					SpecId = specId,
					Name = member.Name,
				};
			Type type = member.DeclaringType;
			Debug.Assert(type != null);
			info.Types.Add(type);
			while (type.IsNested)
			{
				type = type.DeclaringType;
				Debug.Assert(type != null);
				info.Types.Insert(0, type);
			}

			info.Namespace.AddRange(type.Namespace?.Split('.') ?? Enumerable.Empty<string>());

			return info;
		}
	}
}
