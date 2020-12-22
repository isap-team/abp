using System;
using System.Text.Json;

namespace Isap.Abp.Extensions.Data.Specifications
{
	public interface ISpecificationBuilder
	{
		Type EntityType { get; }

		ISpecification Create(ISpecificationBuildingContext context, JsonElement data);
	}

	public interface ISpecificationBuilder<TEntity>: ISpecificationBuilder
	{
		new ISpecification<TEntity> Create(ISpecificationBuildingContext context, JsonElement data);
	}
}
