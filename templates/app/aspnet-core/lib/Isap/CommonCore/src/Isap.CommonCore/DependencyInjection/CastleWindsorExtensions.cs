using Castle.Windsor;

namespace Isap.CommonCore.DependencyInjection
{
	public static class CastleWindsorExtensions
	{
		public static TContainer RegisterContributors<TContainer>(this TContainer container)
			where TContainer: IWindsorContainer
		{
			container.Kernel.ComponentModelBuilder.AddContributor(new PropertyInjectContributeComponentModelConstruction());
			return container;
		}
	}
}
