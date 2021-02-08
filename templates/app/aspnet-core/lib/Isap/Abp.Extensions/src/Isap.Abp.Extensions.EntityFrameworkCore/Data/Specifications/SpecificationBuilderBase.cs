using System;
using Isap.CommonCore.Services;

namespace Isap.Abp.Extensions.Data.Specifications
{
	public abstract class SpecificationBuilderBase<TEntity, TParams>: ISpecificationBuilder<TEntity>
		where TParams: class
	{
		public Type EntityType => typeof(TEntity);

		ISpecification<TEntity> ISpecificationBuilder<TEntity>.Create(ISpecificationBuildingContext context, ISpecificationParametersProvider provider)
		{
			return Create(context, provider.GetParameters<TParams>());
		}

		ISpecification ISpecificationBuilder.Create(ISpecificationBuildingContext context, ISpecificationParametersProvider provider)
		{
			return Create(context, provider.GetParameters<TParams>());
		}

		public abstract ISpecification<TEntity> Create(ISpecificationBuildingContext context, TParams parameters);
	}
}
