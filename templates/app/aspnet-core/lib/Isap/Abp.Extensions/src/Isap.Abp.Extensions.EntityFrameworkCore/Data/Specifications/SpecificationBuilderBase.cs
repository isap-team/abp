using System;
using System.Text.Json;
using Isap.CommonCore.Extensions;

namespace Isap.Abp.Extensions.Data.Specifications
{
	public abstract class SpecificationBuilderBase<TEntity, TParams>: ISpecificationBuilder<TEntity>
		where TParams: class
	{
		public Type EntityType => typeof(TEntity);

		ISpecification<TEntity> ISpecificationBuilder<TEntity>.Create(ISpecificationBuildingContext context, JsonElement data)
		{
			return Create(context, data.ToObject<TParams>());
		}

		ISpecification ISpecificationBuilder.Create(ISpecificationBuildingContext context, JsonElement data)
		{
			return Create(context, data.ToObject<TParams>());
		}

		public abstract ISpecification<TEntity> Create(ISpecificationBuildingContext context, TParams parameters);
	}
}
