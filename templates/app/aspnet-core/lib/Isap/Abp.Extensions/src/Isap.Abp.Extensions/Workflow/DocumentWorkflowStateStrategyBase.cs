using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Domain;
using Isap.CommonCore.Extensions;

namespace Isap.Abp.Extensions.Workflow
{
	public abstract class DocumentWorkflowStateStrategyBase<TDocumentWorkflow, TDocument, TDocumentImpl>: DomainServiceBase, IDocumentWorkflowStateStrategy<TDocumentWorkflow>
		where TDocument: IDocumentEntity
		where TDocumentImpl: TDocument
		where TDocumentWorkflow: IDocumentWorkflow<TDocument>
	{
		public IDocumentWorkflowFactory WorkflowFactory => LazyGetRequiredService<IDocumentWorkflowFactory>();

		Task<bool> IDocumentWorkflowStateStrategy.CanSave(IDocumentWorkflow workflow)
		{
			return CanSave((TDocumentWorkflow) workflow);
		}

		Task IDocumentWorkflowStateStrategy.OnEnter(IDocumentWorkflow workflow, object args)
		{
			return OnEnter((TDocumentWorkflow) workflow);
		}

		Task IDocumentWorkflowStateStrategy.OnReEnter(IDocumentWorkflow workflow, object args)
		{
			return OnReEnter((TDocumentWorkflow) workflow);
		}

		Task IDocumentWorkflowStateStrategy.OnLeave(IDocumentWorkflow workflow)
		{
			return OnLeave((TDocumentWorkflow) workflow);
		}

		Task IDocumentWorkflowStateStrategy<TDocumentWorkflow>.OnEnter(TDocumentWorkflow workflow, object args)
		{
			return OnEnter(workflow);
		}

		Task IDocumentWorkflowStateStrategy<TDocumentWorkflow>.OnReEnter(TDocumentWorkflow workflow, object args)
		{
			return OnReEnter(workflow);
		}

		Task IDocumentWorkflowStateStrategy<TDocumentWorkflow>.OnLeave(TDocumentWorkflow workflow)
		{
			return OnLeave(workflow);
		}

		public virtual Task<bool> CanSave(TDocumentWorkflow workflow)
		{
			return Task.FromResult(true);
		}

		public virtual Task OnEnter(TDocumentWorkflow workflow)
		{
			return Task.CompletedTask;
		}

		public virtual Task OnReEnter(TDocumentWorkflow workflow)
		{
			return Task.CompletedTask;
		}

		public virtual Task OnLeave(TDocumentWorkflow workflow)
		{
			return Task.CompletedTask;
		}

		protected async Task<TDocument> Save(TDocumentWorkflow workflow, Action<TDocumentImpl> update)
		{
			return await workflow.Save(obj => update((TDocumentImpl) obj));
		}
	}

	public abstract class DocumentWorkflowStateStrategyBase<TDocumentWorkflow, TDocument, TDocumentImpl, TArg0>
		: DomainServiceBase, IDocumentWorkflowStateStrategy<TDocumentWorkflow>
		where TDocument: IDocumentEntity
		where TDocumentImpl: TDocument
		where TDocumentWorkflow: IDocumentWorkflow<TDocument>
	{
		public IDocumentWorkflowFactory WorkflowFactory => LazyGetRequiredService<IDocumentWorkflowFactory>();

		Task<bool> IDocumentWorkflowStateStrategy.CanSave(IDocumentWorkflow workflow)
		{
			return CanSave((TDocumentWorkflow) workflow);
		}

		Task IDocumentWorkflowStateStrategy.OnEnter(IDocumentWorkflow workflow, object args)
		{
			return OnEnter((TDocumentWorkflow) workflow, GetArg0(args));
		}

		Task IDocumentWorkflowStateStrategy.OnReEnter(IDocumentWorkflow workflow, object args)
		{
			return OnReEnter((TDocumentWorkflow) workflow, GetArg0(args));
		}

		Task IDocumentWorkflowStateStrategy.OnLeave(IDocumentWorkflow workflow)
		{
			return OnLeave((TDocumentWorkflow) workflow);
		}

		Task IDocumentWorkflowStateStrategy<TDocumentWorkflow>.OnEnter(TDocumentWorkflow workflow, object args)
		{
			TArg0 arg0 = GetArg0(args);
			return OnEnter(workflow, arg0);
		}

		Task IDocumentWorkflowStateStrategy<TDocumentWorkflow>.OnReEnter(TDocumentWorkflow workflow, object args)
		{
			TArg0 arg0 = GetArg0(args);
			return OnReEnter(workflow, arg0);
		}

		Task IDocumentWorkflowStateStrategy<TDocumentWorkflow>.OnLeave(TDocumentWorkflow workflow)
		{
			return OnLeave(workflow);
		}

		public virtual Task<bool> CanSave(TDocumentWorkflow workflow)
		{
			return Task.FromResult(true);
		}

		public virtual Task OnEnter(TDocumentWorkflow workflow, TArg0 arg0)
		{
			return Task.CompletedTask;
		}

		public virtual Task OnReEnter(TDocumentWorkflow workflow, TArg0 arg0)
		{
			return Task.CompletedTask;
		}

		public virtual Task OnLeave(TDocumentWorkflow workflow)
		{
			return Task.CompletedTask;
		}

		private TArg0 GetArg0(object args)
		{
			Dictionary<string, object> map = args.AsNameToObjectMap();
			if (map.TryGetValue("arg0", out object value))
				return Converter.TryConvertTo<TArg0>(value).AsDefaultIfNotSuccess();
			return default;
		}

		protected async Task<TDocument> Save(TDocumentWorkflow workflow, Action<TDocumentImpl> update)
		{
			return await workflow.Save(obj => update((TDocumentImpl) obj));
		}
	}

	public abstract class DocumentWorkflowStateStrategyBase<TDocumentWorkflow, TDocument, TDocumentImpl, TArg0, TArg1>
		: DomainServiceBase, IDocumentWorkflowStateStrategy<TDocumentWorkflow>
		where TDocument: IDocumentEntity
		where TDocumentImpl: TDocument
		where TDocumentWorkflow: IDocumentWorkflow<TDocument>
	{
		public IDocumentWorkflowFactory WorkflowFactory => LazyGetRequiredService<IDocumentWorkflowFactory>();

		Task<bool> IDocumentWorkflowStateStrategy.CanSave(IDocumentWorkflow workflow)
		{
			return CanSave((TDocumentWorkflow) workflow);
		}

		Task IDocumentWorkflowStateStrategy.OnEnter(IDocumentWorkflow workflow, object args)
		{
			var (arg0, arg1) = GetArgs(args);
			return OnEnter((TDocumentWorkflow) workflow, arg0, arg1);
		}

		Task IDocumentWorkflowStateStrategy.OnReEnter(IDocumentWorkflow workflow, object args)
		{
			var (arg0, arg1) = GetArgs(args);
			return OnReEnter((TDocumentWorkflow) workflow, arg0, arg1);
		}

		Task IDocumentWorkflowStateStrategy.OnLeave(IDocumentWorkflow workflow)
		{
			return OnLeave((TDocumentWorkflow) workflow);
		}

		Task IDocumentWorkflowStateStrategy<TDocumentWorkflow>.OnEnter(TDocumentWorkflow workflow, object args)
		{
			var (arg0, arg1) = GetArgs(args);
			return OnEnter(workflow, arg0, arg1);
		}

		Task IDocumentWorkflowStateStrategy<TDocumentWorkflow>.OnReEnter(TDocumentWorkflow workflow, object args)
		{
			var (arg0, arg1) = GetArgs(args);
			return OnReEnter(workflow, arg0, arg1);
		}

		Task IDocumentWorkflowStateStrategy<TDocumentWorkflow>.OnLeave(TDocumentWorkflow workflow)
		{
			return OnLeave(workflow);
		}

		public virtual Task<bool> CanSave(TDocumentWorkflow workflow)
		{
			return Task.FromResult(true);
		}

		public virtual Task OnEnter(TDocumentWorkflow workflow, TArg0 arg0, TArg1 arg1)
		{
			return Task.CompletedTask;
		}

		public virtual Task OnReEnter(TDocumentWorkflow workflow, TArg0 arg0, TArg1 arg1)
		{
			return Task.CompletedTask;
		}

		public virtual Task OnLeave(TDocumentWorkflow workflow)
		{
			return Task.CompletedTask;
		}

		private T GetArg<T>(Dictionary<string, object> args, string argName)
		{
			if (args.TryGetValue(argName, out object value))
				return Converter.TryConvertTo<T>(value).AsDefaultIfNotSuccess();
			return default;
		}

		private (TArg0 arg0, TArg1 arg1) GetArgs(object args)
		{
			Dictionary<string, object> map = args.AsNameToObjectMap();
			return (GetArg<TArg0>(map, "arg0"), GetArg<TArg1>(map, "arg1"));
		}

		protected async Task<TDocument> Save(TDocumentWorkflow workflow, Action<TDocumentImpl> update)
		{
			return await workflow.Save(obj => update((TDocumentImpl) obj));
		}
	}

	public abstract class DocumentWorkflowStateStrategyBase<TDocumentWorkflow, TDocument, TDocumentImpl, TArg0, TArg1, TArg2>
		: DomainServiceBase, IDocumentWorkflowStateStrategy<TDocumentWorkflow>
		where TDocument: IDocumentEntity
		where TDocumentImpl: TDocument
		where TDocumentWorkflow: IDocumentWorkflow<TDocument>
	{
		public IDocumentWorkflowFactory WorkflowFactory => LazyGetRequiredService<IDocumentWorkflowFactory>();

		Task<bool> IDocumentWorkflowStateStrategy.CanSave(IDocumentWorkflow workflow)
		{
			return CanSave((TDocumentWorkflow) workflow);
		}

		Task IDocumentWorkflowStateStrategy.OnEnter(IDocumentWorkflow workflow, object args)
		{
			var (arg0, arg1, arg2) = GetArgs(args);
			return OnEnter((TDocumentWorkflow) workflow, arg0, arg1, arg2);
		}

		Task IDocumentWorkflowStateStrategy.OnReEnter(IDocumentWorkflow workflow, object args)
		{
			var (arg0, arg1, arg2) = GetArgs(args);
			return OnReEnter((TDocumentWorkflow) workflow, arg0, arg1, arg2);
		}

		Task IDocumentWorkflowStateStrategy.OnLeave(IDocumentWorkflow workflow)
		{
			return OnLeave((TDocumentWorkflow) workflow);
		}

		Task IDocumentWorkflowStateStrategy<TDocumentWorkflow>.OnEnter(TDocumentWorkflow workflow, object args)
		{
			var (arg0, arg1, arg2) = GetArgs(args);
			return OnEnter(workflow, arg0, arg1, arg2);
		}

		Task IDocumentWorkflowStateStrategy<TDocumentWorkflow>.OnReEnter(TDocumentWorkflow workflow, object args)
		{
			var (arg0, arg1, arg2) = GetArgs(args);
			return OnReEnter(workflow, arg0, arg1, arg2);
		}

		Task IDocumentWorkflowStateStrategy<TDocumentWorkflow>.OnLeave(TDocumentWorkflow workflow)
		{
			return OnLeave(workflow);
		}

		public virtual Task<bool> CanSave(TDocumentWorkflow workflow)
		{
			return Task.FromResult(true);
		}

		public virtual Task OnEnter(TDocumentWorkflow workflow, TArg0 arg0, TArg1 arg1, TArg2 arg2)
		{
			return Task.CompletedTask;
		}

		public virtual Task OnReEnter(TDocumentWorkflow workflow, TArg0 arg0, TArg1 arg1, TArg2 arg2)
		{
			return Task.CompletedTask;
		}

		public virtual Task OnLeave(TDocumentWorkflow workflow)
		{
			return Task.CompletedTask;
		}

		private T GetArg<T>(Dictionary<string, object> args, string argName)
		{
			if (args.TryGetValue(argName, out object value))
				return Converter.TryConvertTo<T>(value).AsDefaultIfNotSuccess();
			return default;
		}

		private (TArg0 arg0, TArg1 arg1, TArg2 arg2) GetArgs(object args)
		{
			Dictionary<string, object> map = args.AsNameToObjectMap();
			return (GetArg<TArg0>(map, "arg0"), GetArg<TArg1>(map, "arg1"), GetArg<TArg2>(map, "arg2"));
		}

		protected async Task<TDocument> Save(TDocumentWorkflow workflow, Action<TDocumentImpl> update)
		{
			return await workflow.Save(obj => update((TDocumentImpl) obj));
		}
	}
}
