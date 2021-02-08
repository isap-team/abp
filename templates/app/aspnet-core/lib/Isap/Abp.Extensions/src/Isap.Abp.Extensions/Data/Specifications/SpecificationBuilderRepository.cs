using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Isap.Abp.Extensions.Data.Specifications
{
	public interface ISpecificationBuilderRepository
	{
		ISpecificationBuilder<TEntity> Get<TEntity>(Guid specId);
		List<SpecificationMetadata> GetAllMetadata();
	}

	public interface ISpecificationBuilderRepositoryRegistrar
	{
		void Register(Expression<Func<Guid>> specIdExpr, ISpecificationBuilder builder);
	}

	internal class SpecificationBuilderRepository: ISpecificationBuilderRepository, ISpecificationBuilderRepositoryRegistrar
	{
		private readonly Dictionary<Guid, Tuple<SpecificationMetadata, ISpecificationBuilder>> _registeredBuilders =
			new Dictionary<Guid, Tuple<SpecificationMetadata, ISpecificationBuilder>>();

		public ISpecificationBuilder<TEntity> Get<TEntity>(Guid specId)
		{
			if (_registeredBuilders.TryGetValue(specId, out Tuple<SpecificationMetadata, ISpecificationBuilder> metadata))
				if (metadata.Item2.EntityType == typeof(TEntity))
					return (ISpecificationBuilder<TEntity>) metadata.Item2;

			throw new InvalidOperationException($"Specification builder with id = '{specId}' is not registered for entity '{typeof(TEntity).FullName}'.");
		}

		public List<SpecificationMetadata> GetAllMetadata()
		{
			return _registeredBuilders.Values.Select(tuple => tuple.Item1).ToList();
		}

		public void Register(Expression<Func<Guid>> specIdExpr, ISpecificationBuilder builder)
		{
			Guid specId = specIdExpr.Compile().Invoke();

			SpecificationMetadata CreateSpecificationMetadata()
			{
				switch (specIdExpr.Body)
				{
					case MemberExpression memberExpression:
						return SpecificationMetadata.Create(specId, memberExpression.Member);
					default:
						throw new NotSupportedException();
				}
			}

			SpecificationMetadata metadata = CreateSpecificationMetadata();
			_registeredBuilders.Add(metadata.SpecId, Tuple.Create(metadata, builder));
		}
	}
}
