using System;
using Isap.Abp.Extensions.Expressions.Predicates;
using Isap.Converters;
using Volo.Abp.Users;

namespace Isap.Abp.Extensions.Data.Specifications
{
	public interface ISpecificationBuildingContext
	{
		// IIsapDbContextProvider DbContextProvider { get; }
		ISpecificationBuilderRepository SpecificationBuilderRepository { get; }
		IPredicateBuilder PredicateBuilder { get; set; }
		IValueConverter Converter { get; set; }
		ICurrentUser CurrentUser { get; }
		IServiceProvider ServiceProvider { get; set; }
	}
}
