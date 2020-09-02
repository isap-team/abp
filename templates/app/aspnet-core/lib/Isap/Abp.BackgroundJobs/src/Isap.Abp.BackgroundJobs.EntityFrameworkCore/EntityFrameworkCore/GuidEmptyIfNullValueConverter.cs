using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Isap.Abp.BackgroundJobs.EntityFrameworkCore
{
	public class GuidEmptyIfNullValueConverter: ValueConverter<Guid?, Guid>
	{
		public GuidEmptyIfNullValueConverter([CanBeNull] ConverterMappingHints mappingHints = null)
			: base(value => value ?? Guid.Empty, value => value == default ? (Guid?) null : value, mappingHints)
		{
		}
	}
}
