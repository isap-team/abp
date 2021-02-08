using System;
using Isap.CommonCore.Services;

namespace Isap.Abp.Extensions.Data.Specifications
{
	public interface ISpecificationBuilder
	{
		Type EntityType { get; }

		ISpecification Create(ISpecificationBuildingContext context, ISpecificationParametersProvider provider);
	}

	public interface ISpecificationBuilder<TEntity>: ISpecificationBuilder
	{
		new ISpecification<TEntity> Create(ISpecificationBuildingContext context, ISpecificationParametersProvider provider);
	}
}
