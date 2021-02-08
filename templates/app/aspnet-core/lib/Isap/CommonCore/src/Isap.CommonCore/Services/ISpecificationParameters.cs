using System;

namespace Isap.CommonCore.Services
{
	public interface ISpecificationParameters
	{
		Guid SpecId { get; }
		ISpecificationParametersProvider Parameters { get; }
	}
}
