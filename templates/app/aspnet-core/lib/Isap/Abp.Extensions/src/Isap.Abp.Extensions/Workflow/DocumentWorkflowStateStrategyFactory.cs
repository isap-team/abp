using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Factories;
using Isap.CommonCore.Factories;
using Volo.Abp.DependencyInjection;

namespace Isap.Abp.Extensions.Workflow
{
	[AttributeUsage(AttributeTargets.Class)]
	public class DocumentWorkflowStateStrategyAttribute: Attribute, IFactoryProductMarker
	{
		public Type WorkflowType { get; }
		public string DocumentState { get; }

		public DocumentWorkflowStateStrategyAttribute(Type workflowType, string documentState)
		{
			WorkflowType = workflowType;
			DocumentState = documentState;
		}

		string IFactoryProductMarker.ProductKey => DocumentWorkflowStateStrategyFactory.GenerateKey(WorkflowType, DocumentState);
	}

	public interface IDocumentWorkflowStateStrategyFactory: IFactory<IDocumentWorkflowStateStrategy>
	{
		IDocumentWorkflowStateStrategy Create(Type workflowType, string documentState);

		Task Perform(Type workflowType, string documentState, Func<IDocumentWorkflowStateStrategy, Task> action);
		Task<T> Perform<T>(Type workflowType, string documentState, Func<IDocumentWorkflowStateStrategy, Task<T>> action, T defaultValue = default);
	}

	public interface IDocumentWorkflowStateStrategyFactoryBuilder: IIsapFactoryBuilder
	{
	}

	public class DocumentWorkflowStateStrategyFactory
		: IsapFactoryBase<IDocumentWorkflowStateStrategy>, IDocumentWorkflowStateStrategyFactory, IDocumentWorkflowStateStrategyFactoryBuilder
	{
		public static string GenerateKey(Type workflowType, string documentState) => $"{documentState}:{workflowType.FullName}";

		public DocumentWorkflowStateStrategyFactory(ObjectAccessor<IServiceProvider> serviceProviderAccessor)
			: base(serviceProviderAccessor)
		{
		}

		public IDocumentWorkflowStateStrategy Create(Type workflowType, string documentState)
		{
			return Create(GenerateKey(workflowType, documentState));
		}

		public async Task Perform(Type workflowType, string documentState, Func<IDocumentWorkflowStateStrategy, Task> action)
		{
			string productKey = GenerateKey(workflowType, documentState);
			if (IsKnownProduct(productKey))
				using (var wrapper = CreateAsDisposable(productKey))
					await action(wrapper.Object);
		}

		public async Task<T> Perform<T>(Type workflowType, string documentState, Func<IDocumentWorkflowStateStrategy, Task<T>> action, T defaultValue = default)
		{
			string productKey = GenerateKey(workflowType, documentState);
			if (IsKnownProduct(productKey))
				using (var wrapper = CreateAsDisposable(productKey))
					return await action(wrapper.Object);
			return defaultValue;
		}

		protected override void RegisterProducts(Assembly assembly, Action<string, Type> registerProduct)
		{
			List<Tuple<Type, IFactoryProductMarker>> tuples = assembly.GetTypes()
				.Where(t => typeof(IDocumentWorkflowStateStrategy).IsAssignableFrom(t))
				.Select(t => Tuple.Create(t, (IFactoryProductMarker) t.GetCustomAttribute<DocumentWorkflowStateStrategyAttribute>()))
				.Where(tuple => tuple.Item2 != null)
				.ToList();
			foreach (Tuple<Type, IFactoryProductMarker> tuple in tuples)
			{
				registerProduct(tuple.Item2.ProductKey, tuple.Item1);
			}
		}
	}
}
