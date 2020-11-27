using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Domain;
using Isap.Abp.Extensions.Factories;
using Isap.CommonCore.Factories;
using JetBrains.Annotations;
using Volo.Abp.DependencyInjection;

namespace Isap.Abp.Extensions.Workflow
{
	public interface IDocumentWorkflowActivator<out TDocumentWorkflow>
		where TDocumentWorkflow: IDocumentWorkflow
	{
		Task Perform<TDocument>(TDocument document, Func<TDocumentWorkflow, Task> action)
			where TDocument: IDocumentEntity;

		Task<T> Perform<TDocument, T>(TDocument document, Func<TDocumentWorkflow, Task<T>> action)
			where TDocument: IDocumentEntity;
	}

	public interface IDocumentWorkflowFactory: IFactory<IDocumentWorkflow>
	{
		IDocumentWorkflowActivator<TDocumentWorkflow> Create<TDocumentWorkflow>()
			where TDocumentWorkflow: IDocumentWorkflow;
	}

	public interface IDocumentWorkflowFactoryBuilder: IIsapFactoryBuilder
	{
	}

	public class DocumentWorkflowFactory
		: IsapFactoryBase<IDocumentWorkflow>, IDocumentWorkflowFactory, IDocumentWorkflowFactoryBuilder
	{
		private class DocumentWorkflowActivator<TDocumentWorkflow>: IDocumentWorkflowActivator<TDocumentWorkflow>
			where TDocumentWorkflow: IDocumentWorkflow
		{
			private readonly DocumentWorkflowFactory _factory;

			public DocumentWorkflowActivator(DocumentWorkflowFactory factory)
			{
				_factory = factory;
			}

			public async Task Perform<TDocument>([NotNull] TDocument document, Func<TDocumentWorkflow, Task> action) where TDocument: IDocumentEntity
			{
				if (document == null) throw new ArgumentNullException(nameof(document));
				string productKey = GenerateKey(typeof(TDocumentWorkflow));

				if (!_factory.IsKnownProduct(productKey))
					throw new InvalidOperationException($"Can't create workflow for document with type = '{typeof(TDocument).FullName}'.");

				using (var wrapper = _factory.CreateAsDisposable(productKey))
				{
					var workflow = (TDocumentWorkflow) wrapper.Object;
					((IDocumentWorkflowBuilder<TDocument>) workflow).Attach(document);
					await action(workflow);
				}
			}

			public async Task<T> Perform<TDocument, T>([NotNull] TDocument document, Func<TDocumentWorkflow, Task<T>> action) where TDocument: IDocumentEntity
			{
				if (document == null) throw new ArgumentNullException(nameof(document));

				string productKey = GenerateKey(typeof(TDocumentWorkflow));

				if (!_factory.IsKnownProduct(productKey))
					throw new InvalidOperationException($"Can't create workflow for document with type = '{typeof(TDocument).FullName}'.");

				using (var wrapper = _factory.CreateAsDisposable(productKey))
				{
					var workflow = (TDocumentWorkflow) wrapper.Object;
					((IDocumentWorkflowBuilder<TDocument>) workflow).Attach(document);
					return await action(workflow);
				}
			}
		}

		public DocumentWorkflowFactory(ObjectAccessor<IServiceProvider> serviceProviderAccessor)
			: base(serviceProviderAccessor)
		{
		}

		public IDocumentWorkflowActivator<TDocumentWorkflow> Create<TDocumentWorkflow>()
			where TDocumentWorkflow: IDocumentWorkflow
		{
			return new DocumentWorkflowActivator<TDocumentWorkflow>(this);
		}

		public static string GenerateKey(Type documentType) => documentType.FullName;

		protected override void RegisterProducts(Assembly assembly, Action<string, Type> registerProduct)
		{
			List<string> GetProductKeys(Type type)
			{
				return type.GetInterfaces()
					.Where(i => typeof(IDocumentWorkflow).IsAssignableFrom(i))
					.Where(i => i != typeof(IDocumentWorkflow))
					.Select(i => i.FullName)
					.Concat(Enumerable.Repeat(type.FullName, 1))
					.Where(fullName => fullName != null)
					.ToList();
			}

			List<Tuple<Type, List<string>>> tuples = assembly.GetTypes()
				.Where(t => t.IsClass && !t.IsGenericTypeDefinition)
				.Where(t => typeof(IDocumentWorkflow).IsAssignableFrom(t))
				.Select(t => Tuple.Create(t, GetProductKeys(t)))
				.ToList();
			foreach (Tuple<Type, List<string>> tuple in tuples)
			foreach (string productKey in tuple.Item2)
				registerProduct(productKey, tuple.Item1);
		}
	}
}
