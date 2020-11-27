using System;
using System.Threading;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Domain;

namespace Isap.Abp.Extensions.Workflow
{
	public interface IDocumentWorkflow
	{
		string GetState();

		bool CanFire(IStateMachineTrigger trigger);
		Task FireAsync(IStateMachineTrigger trigger, CancellationToken cancellationToken);
		bool CanFire<TArg0>(IStateMachineTrigger<TArg0> trigger);
		Task FireAsync<TArg0>(IStateMachineTrigger<TArg0> trigger, CancellationToken cancellationToken, TArg0 arg0);
		bool CanFire<TArg0, TArg1>(IStateMachineTrigger<TArg0, TArg1> trigger);
		Task FireAsync<TArg0, TArg1>(IStateMachineTrigger<TArg0, TArg1> trigger, CancellationToken cancellationToken, TArg0 arg0, TArg1 arg1);
		bool CanFire<TArg0, TArg1, TArg2>(IStateMachineTrigger<TArg0, TArg1, TArg2> trigger);
		Task FireAsync<TArg0, TArg1, TArg2>(IStateMachineTrigger<TArg0, TArg1, TArg2> trigger, CancellationToken cancellationToken,
			TArg0 arg0, TArg1 arg1, TArg2 arg2);
	}

	public interface IDocumentWorkflow<TDocument>: IDocumentWorkflow
		where TDocument: IDocumentEntity
	{
		TDocument Document { get; }
		Task<TDocument> Save();
		Task<TDocument> Save(Action<object> update);
	}

	public interface IDocumentWorkflow<TDocument, TKey, TDocumentState>: IDocumentWorkflow<TDocument>
		where TDocument: IDocumentEntity<TKey, TDocumentState>
		where TDocumentState: struct
	{
		new TDocumentState GetState();
		Task SaveState(TDocumentState state);
	}

	public interface IDocumentWorkflow<TDocument, out TDocumentImpl, TKey, TDocumentState>: IDocumentWorkflow<TDocument, TKey, TDocumentState>
		where TDocument: IDocumentEntity<TKey, TDocumentState>
		where TDocumentImpl: class, TDocument
		where TDocumentState: struct
	{
		new TDocumentState GetState();
		Task<TDocument> Save(Action<TDocumentImpl> update);
	}
}
