using System;
using Isap.Abp.Extensions.Expressions.Predicates;
using Isap.Converters;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Users;

namespace Isap.Abp.Extensions.Data.Specifications
{
	public class SpecificationBuildingContext: ISpecificationBuildingContext, ITransientDependency
	{
		public IDbSetProvider DbSetProvider { get; set; }
		public ISpecificationBuilderRepository SpecificationBuilderRepository { get; set; }
		public IPredicateBuilder PredicateBuilder { get; set; }
		public IValueConverter Converter { get; set; }
		public ICurrentUser CurrentUser { get; set; }
		public IServiceProvider ServiceProvider { get; set; }
	}
}
