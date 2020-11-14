using System;
using Newtonsoft.Json.Linq;

namespace Isap.Abp.Extensions.Data.Specifications
{
	public abstract class SpecificationBuilderBase<TEntity, TParams>: ISpecificationBuilder<TEntity>
		where TParams: class
	{
		public Type EntityType => typeof(TEntity);

		ISpecification<TEntity> ISpecificationBuilder<TEntity>.Create(ISpecificationBuildingContext context, JToken data)
		{
			return Create(context, data?.ToObject<TParams>());
		}

		ISpecification ISpecificationBuilder.Create(ISpecificationBuildingContext context, JToken data)
		{
			return Create(context, data?.ToObject<TParams>());
		}

		public abstract ISpecification<TEntity> Create(ISpecificationBuildingContext context, TParams parameters);
	}
}
