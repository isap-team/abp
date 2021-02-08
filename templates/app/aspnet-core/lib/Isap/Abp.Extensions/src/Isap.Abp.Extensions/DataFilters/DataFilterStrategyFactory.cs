using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Isap.Abp.Extensions.DataFilters.Concrete;
using Isap.Abp.Extensions.DataFilters.Converters;
using Isap.Abp.Extensions.Expressions.Predicates;
using Isap.Converters;

namespace Isap.Abp.Extensions.DataFilters
{
	public interface IDataFilterStrategyFactory
	{
		IDataFilterStrategy Create(IDataFilterDef dataFilter);
	}

	public interface IDataFilterStrategyFactory<TEntity>: IDataFilterStrategyFactory
	{
		new IDataFilterStrategy<TEntity> Create(IDataFilterDef dataFilter);
	}

	public static class DataFilterStrategyFactory
	{
		private interface IBuilder
		{
			IDataFilterStrategyFactory Build();
		}

		private class Builder<TEntity>: IBuilder
		{
			public Builder(IValueConverter converter, IPredicateBuilder predicateBuilder, IDataFilterValueConverterFactory dataFilterValueConverterFactory,
				ICustomPredicateBuilderFactory customPredicateBuilderFactory)
			{
				Converter = converter;
				PredicateBuilder = predicateBuilder;
				DataFilterValueConverterFactory = dataFilterValueConverterFactory;
				CustomPredicateBuilderFactory = customPredicateBuilderFactory;
			}

			private IValueConverter Converter { get; }
			private IPredicateBuilder PredicateBuilder { get; }
			private IDataFilterValueConverterFactory DataFilterValueConverterFactory { get; }
			private ICustomPredicateBuilderFactory CustomPredicateBuilderFactory { get; }

			IDataFilterStrategyFactory IBuilder.Build()
			{
				return new DataFilterStrategyFactory<TEntity>(Converter, PredicateBuilder, DataFilterValueConverterFactory, CustomPredicateBuilderFactory);
			}
		}

		private static readonly ConcurrentDictionary<Type, IBuilder> _factoryMap = new ConcurrentDictionary<Type, IBuilder>();

		public static IDataFilterStrategyFactory<TEntity> GetOrCreate<TEntity>(IValueConverter converter, IPredicateBuilder predicateBuilder,
			IDataFilterValueConverterFactory dataFilterValueConverterFactory, ICustomPredicateBuilderFactory customPredicateBuilderFactory)
		{
			IBuilder builder = _factoryMap.GetOrAdd(typeof(TEntity),
				type => new Builder<TEntity>(converter, predicateBuilder, dataFilterValueConverterFactory, customPredicateBuilderFactory));
			return (IDataFilterStrategyFactory<TEntity>) builder.Build();
		}

		public static IDataFilterStrategyFactory GetOrCreate(Type entityType, IValueConverter converter, IPredicateBuilder predicateBuilder)
		{
			IBuilder builder = _factoryMap.GetOrAdd(entityType, type =>
				{
					Type builderType = typeof(Builder<>).MakeGenericType(type);
					return (IBuilder) Activator.CreateInstance(builderType, converter, predicateBuilder);
				});
			return builder.Build();
		}
	}

	public class DataFilterStrategyFactory<TEntity>: IDataFilterStrategyFactory<TEntity>
	{
		private class CreationParameters
		{
			public CreationParameters(
				IValueConverter converter,
				IPredicateBuilder predicateBuilder,
				IDataFilterValueConverterFactory dataFilterValueConverterFactory,
				ICustomPredicateBuilderFactory customPredicateBuilderFactory,
				string options)
			{
				Converter = converter;
				PredicateBuilder = predicateBuilder;
				DataFilterValueConverterFactory = dataFilterValueConverterFactory;
				CustomPredicateBuilderFactory = customPredicateBuilderFactory;
				Options = options;
			}

			public IValueConverter Converter { get; }
			public IPredicateBuilder PredicateBuilder { get; }
			public IDataFilterValueConverterFactory DataFilterValueConverterFactory { get; }
			public ICustomPredicateBuilderFactory CustomPredicateBuilderFactory { get; }
			public string Options { get; }
		}

		private readonly Dictionary<DataFilterType, Func<CreationParameters, IDataFilterStrategy<TEntity>>> _ctorMap =
			new Dictionary<DataFilterType, Func<CreationParameters, IDataFilterStrategy<TEntity>>>
				{
					{
						DataFilterType.SelectOne,
						@params => new SelectOneDataFilterStrategy<TEntity>(DataFilterOptionsExtensions.Deserialize(@params.Options))
							{
								Converter = @params.Converter,
								PredicateBuilder = @params.PredicateBuilder,
								DataFilterValueConverterFactory = @params.DataFilterValueConverterFactory,
								CustomPredicateBuilderFactory = @params.CustomPredicateBuilderFactory,
							}
					},
					{
						DataFilterType.SelectMany,
						@params => new SelectManyDataFilterStrategy<TEntity>(DataFilterOptionsExtensions.Deserialize(@params.Options))
							{
								Converter = @params.Converter,
								PredicateBuilder = @params.PredicateBuilder,
								DataFilterValueConverterFactory = @params.DataFilterValueConverterFactory,
								CustomPredicateBuilderFactory = @params.CustomPredicateBuilderFactory,
							}
					},
					{
						DataFilterType.SelectRange,
						@params => new SelectRangeDataFilterStrategy<TEntity>(DataFilterOptionsExtensions.Deserialize(@params.Options))
							{
								Converter = @params.Converter,
								PredicateBuilder = @params.PredicateBuilder,
								DataFilterValueConverterFactory = @params.DataFilterValueConverterFactory,
								CustomPredicateBuilderFactory = @params.CustomPredicateBuilderFactory,
							}
					},
					{
						DataFilterType.SearchLike,
						@params => new SearchLikeDataFilterStrategy<TEntity>(DataFilterOptionsExtensions.Deserialize(@params.Options))
							{
								Converter = @params.Converter,
								PredicateBuilder = @params.PredicateBuilder,
								DataFilterValueConverterFactory = @params.DataFilterValueConverterFactory,
								CustomPredicateBuilderFactory = @params.CustomPredicateBuilderFactory,
							}
					},
					{
						DataFilterType.IsAnyOrOne,
						@params => new IsAnyOrOneDataFilterStrategy<TEntity>(DataFilterOptionsExtensions.Deserialize(@params.Options))
							{
								Converter = @params.Converter,
								PredicateBuilder = @params.PredicateBuilder,
								DataFilterValueConverterFactory = @params.DataFilterValueConverterFactory,
								CustomPredicateBuilderFactory = @params.CustomPredicateBuilderFactory,
							}
					},
					{
						DataFilterType.IsAnyOrRange,
						@params => new IsAnyOrRangeDataFilterStrategy<TEntity>(DataFilterOptionsExtensions.Deserialize(@params.Options))
							{
								Converter = @params.Converter,
								PredicateBuilder = @params.PredicateBuilder,
								DataFilterValueConverterFactory = @params.DataFilterValueConverterFactory,
								CustomPredicateBuilderFactory = @params.CustomPredicateBuilderFactory,
							}
					},
					{
						DataFilterType.SearchLikeWithExact,
						@params => new SearchLikeExactDataFilterStrategy<TEntity>(DataFilterOptionsExtensions.Deserialize(@params.Options))
							{
								Converter = @params.Converter,
								PredicateBuilder = @params.PredicateBuilder,
								DataFilterValueConverterFactory = @params.DataFilterValueConverterFactory,
								CustomPredicateBuilderFactory = @params.CustomPredicateBuilderFactory,
							}
					},
					{
						DataFilterType.SearchLikeSpace,
						@params => new SearchLikePhoneDataFilterStrategy<TEntity>(DataFilterOptionsExtensions.Deserialize(@params.Options))
							{
								Converter = @params.Converter,
								PredicateBuilder = @params.PredicateBuilder,
								DataFilterValueConverterFactory = @params.DataFilterValueConverterFactory,
								CustomPredicateBuilderFactory = @params.CustomPredicateBuilderFactory,
							}
					},
				};

		internal DataFilterStrategyFactory(IValueConverter converter, IPredicateBuilder predicateBuilder,
			IDataFilterValueConverterFactory dataFilterValueConverterFactory, ICustomPredicateBuilderFactory customPredicateBuilderFactory)
		{
			Converter = converter;
			PredicateBuilder = predicateBuilder;
			DataFilterValueConverterFactory = dataFilterValueConverterFactory;
			CustomPredicateBuilderFactory = customPredicateBuilderFactory;
		}

		public IValueConverter Converter { get; }
		public IPredicateBuilder PredicateBuilder { get; }
		public IDataFilterValueConverterFactory DataFilterValueConverterFactory { get; }
		public ICustomPredicateBuilderFactory CustomPredicateBuilderFactory { get; }

		public IDataFilterStrategy<TEntity> Create(IDataFilterDef dataFilter)
		{
			if (!string.Equals(typeof(TEntity).AssemblyQualifiedName, dataFilter.TargetEntityType, StringComparison.Ordinal))
				throw new NotSupportedException(
					$"Data filter can't be used with entity type '{typeof(TEntity).AssemblyQualifiedName}' because it was defined for target entity type = '{dataFilter.TargetEntityType}'.");
			if (!_ctorMap.TryGetValue(dataFilter.Type, out var ctor))
				throw new NotSupportedException($"Data filter type value '{dataFilter.Type} is not supported yet.");
			var parameters = new CreationParameters(Converter, PredicateBuilder, DataFilterValueConverterFactory, CustomPredicateBuilderFactory,
				dataFilter.Options);
			return ctor(parameters);
		}

		IDataFilterStrategy IDataFilterStrategyFactory.Create(IDataFilterDef dataFilter)
		{
			return Create(dataFilter);
		}
	}
}
