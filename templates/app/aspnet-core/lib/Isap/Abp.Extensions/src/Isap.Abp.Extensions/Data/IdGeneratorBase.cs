using System;
using Volo.Abp.Guids;

namespace Isap.Abp.Extensions.Data
{
	public abstract class IdGeneratorBase<T>: IIdGenerator<T>
	{
		public T NextValue()
		{
			return InternalNextValue();
		}

		object IIdGenerator.NextValue()
		{
			return NextValue();
		}

		protected abstract T InternalNextValue();
	}

	public class GuidIdGenerator: IdGeneratorBase<Guid>
	{
		public GuidIdGenerator(IGuidGenerator guidGenerator)
		{
			GuidGenerator = guidGenerator;
		}

		public IGuidGenerator GuidGenerator { get; }

		protected override Guid InternalNextValue()
		{
			return GuidGenerator.Create();
		}
	}
}
