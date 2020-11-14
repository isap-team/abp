using System;
using System.Collections.Generic;

namespace Isap.Abp.Extensions.Data.Specifications
{
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
}
