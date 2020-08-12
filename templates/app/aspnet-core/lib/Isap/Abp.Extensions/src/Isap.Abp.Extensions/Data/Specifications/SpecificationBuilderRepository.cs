using System;
using System.Collections.Generic;
using System.Linq;
using Isap.Abp.Extensions.Data.Specifications.FilterSpecs;
using Isap.Abp.Extensions.Data.Specifications.OrderSpecs;
using Isap.CommonCore.Services;

namespace Isap.Abp.Extensions.Data.Specifications
{
	public interface ISpecificationBuilderRepositoryRegistrationConsumer
	{
		void RegisterBuilders(ISpecificationBuilderRepositoryRegistrar repository);
	}

	public interface ISpecificationBuilderRepository
	{
		ISpecificationBuilder<TEntity> Get<TEntity>(Guid specId);
	}

	public interface ISpecificationBuilderRepositoryRegistrar
	{
		void Register(Guid specId, ISpecificationBuilder builder);
	}

	internal class SpecificationBuilderRepository: ISpecificationBuilderRepository, ISpecificationBuilderRepositoryRegistrar
	{
		private readonly Dictionary<Guid, ISpecificationBuilder> _registeredBuilders = new Dictionary<Guid, ISpecificationBuilder>();

		public ISpecificationBuilder<TEntity> Get<TEntity>(Guid specId)
		{
			if (_registeredBuilders.TryGetValue(specId, out ISpecificationBuilder builder))
				if (builder.EntityType == typeof(TEntity))
					return (ISpecificationBuilder<TEntity>) builder;

			throw new InvalidOperationException($"Specification builder with id = '{specId}' is not registered for entity '{typeof(TEntity).FullName}'.");
		}

		public void Register(Guid specId, ISpecificationBuilder builder)
		{
			_registeredBuilders.Add(specId, builder);
		}
	}

	public static class SpecificationHelpers
	{
		public static List<ISpecification<TEntity>> BuildSpecifications<TEntity>(ISpecificationBuildingContext context,
			IIsapDbContextProvider dbContextProvider, ICollection<SpecificationParameters> specifications)
		{
			if (specifications == null)
				return new List<ISpecification<TEntity>>();

			ISpecification<TEntity> ToSpec(SpecificationParameters specification)
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
			IIsapDbContextProvider dbContextProvider, ICollection<SpecificationParameters> specifications)
		{
			var specs = BuildSpecifications<TEntity>(context, dbContextProvider, specifications);
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
			IIsapDbContextProvider dbContextProvider, ICollection<SpecificationParameters> specifications)
		{
			var specs = BuildSpecifications<TEntity>(context, dbContextProvider, specifications);
			return specs.ToOrderSpecification();
		}
	}
}
