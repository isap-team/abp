using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Isap.CommonCore.DependencyInjection;
using Xunit;

namespace Isap.CommonCore.Tests.DependencyInjection
{
	public class DependencyHelpersTest
	{
		public interface ISomeEntity
		{
		}

		public interface IOtherEntity
		{
			ISomeEntity SomeEntity { get; set; }
		}

		public class ClassA: ISomeEntity
		{
		}

		public class ClassB: ISomeEntity
		{
		}

		public class OtherClassA: IOtherEntity
		{
			[PropertyInject(DependencyKey = "A", IsOptional = false)]
			public ISomeEntity SomeEntity { get; set; }
		}

		public class OtherClassB: IOtherEntity
		{
			[PropertyInject(DependencyKey = "B", IsOptional = false)]
			public ISomeEntity SomeEntity { get; set; }
		}

		[Fact]
		public void ResolveDependencyTest()
		{
			var container = new WindsorContainer().RegisterContributors();
			container.Kernel.Resolver.AddSubResolver(new PropertyDependencyResolver(container));
			container.Register(new IRegistration[]
				{
					Component.For<ISomeEntity>()
						.NamedDependency("A")
						.UsingFactoryMethod(() => new ClassA())
						.LifestyleTransient(),
					Component.For<ISomeEntity>()
						.NamedDependency("B")
						.UsingFactoryMethod(() => new ClassB())
						.LifestyleTransient(),
					Component.For<IOtherEntity>()
						.NamedDependency("A")
						.ImplementedBy<OtherClassA>()
						.LifestyleTransient(),
					Component.For<IOtherEntity>()
						.NamedDependency("B")
						.ImplementedBy<OtherClassB>()
						.LifestyleTransient(),
				});
			Assert.IsType<ClassA>(container.Resolve<ISomeEntity>());
			Assert.IsType<ClassA>(container.ResolveDependency<ISomeEntity>("A"));
			Assert.IsType<ClassB>(container.ResolveDependency<ISomeEntity>("B"));

			IOtherEntity otherEntityA = container.ResolveDependency<IOtherEntity>("A");
			Assert.IsType<OtherClassA>(otherEntityA);
			Assert.NotNull(otherEntityA.SomeEntity);
			Assert.IsType<ClassA>(otherEntityA.SomeEntity);

			IOtherEntity otherEntityB = container.ResolveDependency<IOtherEntity>("B");
			Assert.IsType<OtherClassB>(otherEntityB);
			Assert.NotNull(otherEntityB.SomeEntity);
			Assert.IsType<ClassB>(otherEntityB.SomeEntity);
		}
	}
}
