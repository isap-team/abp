using System;
using Newtonsoft.Json.Linq;

namespace Isap.Abp.Extensions.Data.Specifications
{
	public interface ISpecificationBuilder
	{
		Type EntityType { get; }

		ISpecification Create(ISpecificationBuildingContext context, JToken data);
	}

	public interface ISpecificationBuilder<TEntity>: ISpecificationBuilder
	{
		new ISpecification<TEntity> Create(ISpecificationBuildingContext context, JToken data);
	}
}
