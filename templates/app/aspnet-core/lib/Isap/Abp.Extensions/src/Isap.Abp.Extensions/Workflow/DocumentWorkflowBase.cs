using System;
using System.Threading;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Domain;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Isap.Abp.Extensions.Workflow
{
	public interface IDocumentWorkflowBuilder<in TDocument>
		where TDocument: IDocumentEntity
	{
		void Attach(TDocument document);
	}

	public abstract class DocumentWorkflowBase<TDocument, TDocumentImpl, TKey, TDocumentState, TStateMachine>
		: DomainServiceBase, IDocumentWorkflow<TDocument, TDocumentImpl, TKey, TDocumentState>, IDocumentWorkflowBuilder<TDocument>
		where TDocument: class, IDocumentEntity<TKey, TDocumentState>
		where TDocumentImpl: class, TDocument
		where TDocumentState: struct
		where TStateMachine: IDocumentWorkflowStateMachine
	{
		public TDocument Document { get; private set; }

		public TStateMachine StateMachine { get; set; }

		string IDocumentWorkflow.GetState()
		{
			return Document.State.ToString();
		}

		public TDocumentState GetState()
		{
			return Document.State;
		}

		public bool CanFire(IStateMachineTrigger trigger)
		{
			return StateMachine.CanFire(this, trigger);
		}

		public async Task FireAsync(IStateMachineTrigger trigger, CancellationToken cancellationToken)
		{
			await StateMachine.FireAsync(this, trigger, cancellationToken);
		}

		public bool CanFire<TArg0>(IStateMachineTrigger<TArg0> trigger)
		{
			return StateMachine.CanFire(this, trigger);
		}

		public async Task FireAsync<TArg0>(IStateMachineTrigger<TArg0> trigger, CancellationToken cancellationToken, TArg0 arg0)
		{
			await StateMachine.FireAsync(this, trigger, arg0, cancellationToken);
		}

		public bool CanFire<TArg0, TArg1>(IStateMachineTrigger<TArg0, TArg1> trigger)
		{
			return StateMachine.CanFire(this, trigger);
		}

		public async Task FireAsync<TArg0, TArg1>(IStateMachineTrigger<TArg0, TArg1> trigger, CancellationToken cancellationToken, TArg0 arg0, TArg1 arg1)
		{
			await StateMachine.FireAsync(this, trigger, arg0, arg1, cancellationToken);
		}

		public bool CanFire<TArg0, TArg1, TArg2>(IStateMachineTrigger<TArg0, TArg1, TArg2> trigger)
		{
			return StateMachine.CanFire(this, trigger);
		}

		public async Task FireAsync<TArg0, TArg1, TArg2>(IStateMachineTrigger<TArg0, TArg1, TArg2> trigger,
			CancellationToken cancellationToken, TArg0 arg0, TArg1 arg1, TArg2 arg2)
		{
			await StateMachine.FireAsync(this, trigger, arg0, arg1, arg2, cancellationToken);
		}

		async Task IDocumentWorkflow<TDocument, TKey, TDocumentState>.SaveState(TDocumentState state)
		{
			Document = await SaveState(state);
		}

		public async Task<TDocument> Save()
		{
			Document = await Save(Document);
			return Document;
		}

		public async Task<TDocument> Save(Action<object> update)
		{
			Document = await Save(Document, obj => update(obj));
			return Document;
		}

		public async Task<TDocument> Save(Action<TDocumentImpl> update)
		{
			Document = await Save(Document, obj => update(obj));
			return Document;
		}

		protected abstract Task<TDocument> SaveState(TDocumentState state);
		protected abstract Task<TDocument> Save(TDocument document);
		protected abstract Task<TDocument> Save(TDocument document, Action<TDocumentImpl> update);

		void IDocumentWorkflowBuilder<TDocument>.Attach(TDocument document) => Attach(document);

		protected virtual void Attach(TDocument document)
		{
			Document = document;
		}

		protected async Task<TDocument> Do(Task task, Func<TDocument, Task<TDocument>> complete = default)
		{
			await task;
			if (complete != default)
				Document = await complete(Document);
			return Document;
		}
	}

	public abstract class DocumentWorkflowBase<TDocument, TDocumentImpl, TKey, TDocumentState, TStateMachine, TDomainManager>
		: DocumentWorkflowBase<TDocument, TDocumentImpl, TKey, TDocumentState, TStateMachine>
		where TDocument: class, IDocumentEntity<TKey, TDocumentState>
		where TDocumentImpl: class, IDocumentEntity<TKey, TDocumentState>, TDocument, IEntity<TKey> where TDocumentState: struct
		where TStateMachine: IDocumentWorkflowStateMachine
		where TDomainManager: IDomainManager<TDocument, TDocumentImpl, TKey>
	{
		public TDomainManager DomainManager => LazyGetRequiredService<TDomainManager>();

		protected override async Task<TDocument> SaveState(TDocumentState state)
		{
			return await DomainManager.Save(Document.Id, e => e.State = state);
		}

		protected override async Task<TDocument> Save(TDocument document)
		{
			TDocument existingDocument = await DomainManager.Get(document.Id);
			TDocumentImpl editableDocument = DomainManager.AsEditable(document);
			if (existingDocument != null)
			{
				Attach(existingDocument);
				if (!await StateMachine.CanSave(this))
					throw new UserFriendlyException("Changing document properties is not allowed on current document state.");
			}

			return await DomainManager.Save(editableDocument);
		}

		protected override async Task<TDocument> Save(TDocument document, Action<TDocumentImpl> update)
		{
			return await DomainManager.Save(document, update);
		}
	}
}
