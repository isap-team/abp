using System.Collections.Generic;
using System.Linq;
using Isap.Abp.Extensions.Data.Specifications.FilterSpecs;
using Isap.Abp.Extensions.Data.Specifications.OrderSpecs;
using Isap.CommonCore.Services;

namespace Isap.Abp.Extensions.Data.Specifications
{
	public static class SpecificationHelpers
	{
		public static List<ISpecification<TEntity>> BuildSpecifications<TEntity>(ISpecificationBuildingContext context,
			IEnumerable<ISpecificationParameters> specifications)
		{
			if (specifications == null)
				return new List<ISpecification<TEntity>>();

			ISpecification<TEntity> ToSpec(ISpecificationParameters specification)
			{
				ISpecificationBuilder<TEntity> builder = context.SpecificationBuilderRepository.Get<TEntity>(specification.SpecId);
				return builder.Create(context, specification.Parameters);
			}

			return specifications.Select(ToSpec).ToList();
		}

		public static IFilterSpecification<TEntity> ToFilterSpecification<TEntity>(this IEnumerable<ISpecification<TEntity>> specifications)
		{
			IFilterSpecification<TEntity>[] filterSpecs = specifications.OfType<IFilterSpecification<TEntity>>().ToArray();
			if (filterSpecs.Length == 0)
				return new TrueSpecification<TEntity>();
			return new AndSpecification<TEntity>(filterSpecs);
		}

		public static IFilterSpecification<TEntity> BuildFilterSpecification<TEntity>(ISpecificationBuildingContext context,
			IEnumerable<ISpecificationParameters> specifications)
		{
			var specs = BuildSpecifications<TEntity>(context, specifications);
			return specs.ToFilterSpecification();
		}

		public static IOrderSpecification<TEntity> ToOrderSpecification<TEntity>(this IEnumerable<ISpecification<TEntity>> specifications)
		{
			IOrderSpecification<TEntity>[] orderSpecs = specifications.OfType<IOrderSpecification<TEntity>>().ToArray();
			if (orderSpecs.Length == 0)
				return new NoOrderBySpecification<TEntity>();
			return new ConsolidatedOrderBySpecification<TEntity>(orderSpecs);
		}

		public static IOrderSpecification<TEntity> BuildOrderSpecification<TEntity>(ISpecificationBuildingContext context,
			IEnumerable<ISpecificationParameters> specifications)
		{
			var specs = BuildSpecifications<TEntity>(context, specifications);
			return specs.ToOrderSpecification();
		}
	}
}
