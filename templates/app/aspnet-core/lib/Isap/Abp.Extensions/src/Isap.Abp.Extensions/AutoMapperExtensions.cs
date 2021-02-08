using System;
using AutoMapper;

namespace Isap.Abp.Extensions
{
	public class AutoMapperExtensions<TSource, TDestination, TMember>
	{
		private readonly IMemberConfigurationExpression<TSource, TDestination, TMember> _options;

		public AutoMapperExtensions(IMemberConfigurationExpression<TSource, TDestination, TMember> options)
		{
			_options = options;
		}

		/// <summary>
		///     Выполняет мапирование связанной сущности.
		/// </summary>
		/// <typeparam name="TSourceMember"></typeparam>
		/// <param name="mappingFunction"></param>
		public void MapFrom<TSourceMember>(Func<TSource, TSourceMember> mappingFunction)
		{
			_options.MapFrom((src, dest, member, res) => res.Mapper.Map<TSourceMember, TMember>(mappingFunction(src)));
		}
	}

	public static class AutoMapperExtensions
	{
		public static AutoMapperExtensions<TSource, TDestination, TMember> Extensions<TSource, TDestination, TMember>(
			this IMemberConfigurationExpression<TSource, TDestination, TMember> expression)
		{
			return new AutoMapperExtensions<TSource, TDestination, TMember>(expression);
		}

		public static object GetService(this ResolutionContext context, Type serviceType)
		{
			return context.Options.ServiceCtor(serviceType);
		}

		public static T GetService<T>(this ResolutionContext context)
		{
			return (T) context.GetService(typeof(T));
		}

		public static object GetRequiredService(this ResolutionContext context, Type serviceType)
		{
			object service = GetService(context, serviceType);
			if (service == null)
				throw new InvalidOperationException($"Required service with type = '{serviceType}' is not resolved.");
			return service;
		}

		public static T GetRequiredService<T>(this ResolutionContext context)
		{
			return (T) GetRequiredService(context, typeof(T));
		}

		public static TDestination LoadInstance<TService, TDestination>(this ResolutionContext context, object source, Func<TService, TDestination> load)
		{
			string key = $"@{typeof(TDestination)}:{source}";
			if (context.Options.Items.TryGetValue(key, out object result))
				return (TDestination) result;
			TDestination instance = load(context.GetRequiredService<TService>());
			context.Options.Items[key] = instance;
			return instance;
		}

		public static void CustomMapFrom<TSource, TDestination, TSourceMember, TMember>(this IMemberConfigurationExpression<TSource, TDestination, TMember> opt,
			Func<TSource, TSourceMember> getSourceMember, Func<TSource, TSourceMember, TDestination, TMember> createMember)
			where TMember: class
		{
			opt.MapFrom((source, destination, member, resolutionContext) =>
				{
					TSourceMember sourceMember = getSourceMember(source);
					TMember destinationMember = member ?? createMember(source, sourceMember, destination);
					return resolutionContext.Mapper.Map(sourceMember, destinationMember);
				});
		}

		public static void CustomMapFrom<TSource, TDestination, TSourceMember, TMember>(this IMemberConfigurationExpression<TSource, TDestination, TMember> opt,
			Func<TSource, TSourceMember> getSourceMember, Func<TSourceMember, TDestination, TMember> createMember)
			where TMember: class
		{
			opt.MapFrom((source, destination, member, resolutionContext) =>
				{
					TSourceMember sourceMember = getSourceMember(source);
					TMember destinationMember = member ?? createMember(sourceMember, destination);
					return resolutionContext.Mapper.Map(sourceMember, destinationMember);
				});
		}

		public static void CustomMapFrom<TSource, TDestination, TSourceMember, TMember>(this IMemberConfigurationExpression<TSource, TDestination, TMember> opt,
			Func<TSource, TSourceMember> getSourceMember, Func<TDestination, TMember> createMember)
			where TMember: class
		{
			opt.MapFrom((source, destination, member, resolutionContext) =>
				{
					TSourceMember sourceMember = getSourceMember(source);
					TMember destinationMember = member ?? createMember(destination);
					return resolutionContext.Mapper.Map(sourceMember, destinationMember);
				});
		}
	}
}
