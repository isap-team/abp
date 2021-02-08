using System;
using System.Threading;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Domain;
using Isap.Converters;
using Stateless;
using Volo.Abp.Threading;

namespace Isap.Abp.Extensions.Workflow
{
	public interface IDocumentWorkflowStateMachine
	{
		Task<bool> CanSave(IDocumentWorkflow workflow);

		bool CanFire(IDocumentWorkflow workflow, IStateMachineTrigger trigger);
		Task FireAsync(IDocumentWorkflow workflow, IStateMachineTrigger trigger, CancellationToken cancellationToken);

		bool CanFire<TArg0>(IDocumentWorkflow workflow, IStateMachineTrigger<TArg0> trigger);
		Task FireAsync<TArg0>(IDocumentWorkflow workflow, IStateMachineTrigger<TArg0> trigger, TArg0 arg0, CancellationToken cancellationToken);

		bool CanFire<TArg0, TArg1>(IDocumentWorkflow workflow, IStateMachineTrigger<TArg0, TArg1> trigger);

		Task FireAsync<TArg0, TArg1>(IDocumentWorkflow workflow, IStateMachineTrigger<TArg0, TArg1> trigger, TArg0 arg0, TArg1 arg1,
			CancellationToken cancellationToken);

		bool CanFire<TArg0, TArg1, TArg2>(IDocumentWorkflow workflow, IStateMachineTrigger<TArg0, TArg1, TArg2> trigger);

		Task FireAsync<TArg0, TArg1, TArg2>(IDocumentWorkflow workflow, IStateMachineTrigger<TArg0, TArg1, TArg2> trigger, TArg0 arg0, TArg1 arg1, TArg2 arg2,
			CancellationToken cancellationToken);
	}

	public abstract class DocumentWorkflowStateMachineBase: IDocumentWorkflowStateMachine
	{
		private readonly AsyncLocalStackContainer<IDocumentWorkflow> _workflowStackContainer =
			new AsyncLocalStackContainer<IDocumentWorkflow>(() => null);

		protected DocumentWorkflowStateMachineBase()
		{
			Converter = ValueConverterProviders.Current.GetConverter();
		}

		protected IDocumentWorkflow Current => _workflowStackContainer.Current;

		public IValueConverter Converter { get; set; }

		public abstract Task<bool> CanSave(IDocumentWorkflow workflow);

		public abstract bool CanFire(IDocumentWorkflow workflow, IStateMachineTrigger trigger);
		public abstract Task FireAsync(IDocumentWorkflow workflow, IStateMachineTrigger trigger, CancellationToken cancellationToken);

		public abstract bool CanFire<TArg0>(IDocumentWorkflow workflow, IStateMachineTrigger<TArg0> trigger);
		public abstract Task FireAsync<TArg0>(IDocumentWorkflow workflow, IStateMachineTrigger<TArg0> trigger, TArg0 arg0, CancellationToken cancellationToken);

		public abstract bool CanFire<TArg0, TArg1>(IDocumentWorkflow workflow, IStateMachineTrigger<TArg0, TArg1> trigger);

		public abstract Task FireAsync<TArg0, TArg1>(IDocumentWorkflow workflow, IStateMachineTrigger<TArg0, TArg1> trigger, TArg0 arg0, TArg1 arg1,
			CancellationToken cancellationToken);

		public abstract bool CanFire<TArg0, TArg1, TArg2>(IDocumentWorkflow workflow, IStateMachineTrigger<TArg0, TArg1, TArg2> trigger);

		public abstract Task FireAsync<TArg0, TArg1, TArg2>(IDocumentWorkflow workflow, IStateMachineTrigger<TArg0, TArg1, TArg2> trigger, TArg0 arg0,
			TArg1 arg1, TArg2 arg2, CancellationToken cancellationToken);

		public IDisposable Use(IDocumentWorkflow workflow)
		{
			return _workflowStackContainer.Use(baseProvider => workflow);
		}
	}

	public abstract class DocumentWorkflowStateMachineBase<TDocumentState, TDocumentTrigger>: DocumentWorkflowStateMachineBase
	{
		public class StateMachineTrigger: IStateMachineTrigger
		{
			private readonly StateMachine<TDocumentState, TDocumentTrigger> _stateMachine;

			public StateMachineTrigger(StateMachine<TDocumentState, TDocumentTrigger> stateMachine, TDocumentTrigger trigger)
			{
				_stateMachine = stateMachine;
				Trigger = trigger;
			}

			public TDocumentTrigger Trigger { get; }

			public bool CanFire() => _stateMachine.CanFire(Trigger);

			public Task FireAsync(CancellationToken cancellationToken) => Task.Run(() => _stateMachine.FireAsync(Trigger), cancellationToken);
		}

		public class StateMachineTrigger<TArg0>: IStateMachineTrigger<TArg0>
		{
			private readonly StateMachine<TDocumentState, TDocumentTrigger> _stateMachine;

			public StateMachineTrigger(StateMachine<TDocumentState, TDocumentTrigger> stateMachine, TDocumentTrigger trigger)
			{
				_stateMachine = stateMachine;
				Trigger = stateMachine.SetTriggerParameters<TArg0>(trigger);
			}

			public StateMachine<TDocumentState, TDocumentTrigger>.TriggerWithParameters<TArg0> Trigger { get; }

			public bool CanFire() => _stateMachine.CanFire(Trigger.Trigger);

			public Task FireAsync(CancellationToken cancellationToken, TArg0 arg0) => Task.Run(() => _stateMachine.FireAsync(Trigger, arg0), cancellationToken);
		}

		public class StateMachineTrigger<TArg0, TArg1>: IStateMachineTrigger<TArg0, TArg1>
		{
			private readonly StateMachine<TDocumentState, TDocumentTrigger> _stateMachine;

			public StateMachineTrigger(StateMachine<TDocumentState, TDocumentTrigger> stateMachine, TDocumentTrigger trigger)
			{
				_stateMachine = stateMachine;
				Trigger = stateMachine.SetTriggerParameters<TArg0, TArg1>(trigger);
			}

			public StateMachine<TDocumentState, TDocumentTrigger>.TriggerWithParameters<TArg0, TArg1> Trigger { get; }

			public bool CanFire() => _stateMachine.CanFire(Trigger.Trigger);

			public Task FireAsync(CancellationToken cancellationToken, TArg0 arg0, TArg1 arg1) =>
				Task.Run(() => _stateMachine.FireAsync(Trigger, arg0, arg1), cancellationToken);
		}

		public class StateMachineTrigger<TArg0, TArg1, TArg2>: IStateMachineTrigger<TArg0, TArg1, TArg2>
		{
			private readonly StateMachine<TDocumentState, TDocumentTrigger> _stateMachine;

			public StateMachineTrigger(StateMachine<TDocumentState, TDocumentTrigger> stateMachine, TDocumentTrigger trigger)
			{
				_stateMachine = stateMachine;
				Trigger = stateMachine.SetTriggerParameters<TArg0, TArg1, TArg2>(trigger);
			}

			public StateMachine<TDocumentState, TDocumentTrigger>.TriggerWithParameters<TArg0, TArg1, TArg2> Trigger { get; }

			public bool CanFire() => _stateMachine.CanFire(Trigger.Trigger);

			public Task FireAsync(CancellationToken cancellationToken, TArg0 arg0, TArg1 arg1, TArg2 arg2) =>
				Task.Run(() => _stateMachine.FireAsync(Trigger, arg0, arg1, arg2), cancellationToken);
		}

		protected class StateMachineConfiguration
		{
			private readonly DocumentWorkflowStateMachineBase<TDocumentState, TDocumentTrigger> _workflowStateMachine;
			private readonly StateMachine<TDocumentState, TDocumentTrigger> _stateMachine;

			public StateMachineConfiguration(DocumentWorkflowStateMachineBase<TDocumentState, TDocumentTrigger> workflowStateMachine,
				StateMachine<TDocumentState, TDocumentTrigger> stateMachine)
			{
				_workflowStateMachine = workflowStateMachine;
				_stateMachine = stateMachine;
			}

			public StateMachine<TDocumentState, TDocumentTrigger>.StateConfiguration Configure(TDocumentState state)
			{
				return _stateMachine
						.Configure(state)
						.OnExitAsync(_workflowStateMachine.OnExitAsync)
					;
			}

			public StateMachineTrigger CreateTrigger(TDocumentTrigger trigger)
			{
				return new StateMachineTrigger(_stateMachine, trigger);
			}

			public StateMachineTrigger<TArg0> CreateTrigger<TArg0>(TDocumentTrigger trigger)
			{
				return new StateMachineTrigger<TArg0>(_stateMachine, trigger);
			}

			public StateMachineTrigger<TArg0, TArg1> CreateTrigger<TArg0, TArg1>(TDocumentTrigger trigger)
			{
				return new StateMachineTrigger<TArg0, TArg1>(_stateMachine, trigger);
			}

			public StateMachineTrigger<TArg0, TArg1, TArg2> CreateTrigger<TArg0, TArg1, TArg2>(TDocumentTrigger trigger)
			{
				return new StateMachineTrigger<TArg0, TArg1, TArg2>(_stateMachine, trigger);
			}
		}

		protected DocumentWorkflowStateMachineBase()
		{
			StateMachine = CreateStateMachine();
		}

		protected StateMachine<TDocumentState, TDocumentTrigger> StateMachine { get; }

		public IDocumentWorkflowStateStrategyFactory StrategyFactory { get; set; }

		public override async Task<bool> CanSave(IDocumentWorkflow workflow)
		{
			return await StrategyFactory.Perform(workflow.GetType(), workflow.GetState(), strategy => strategy.CanSave(workflow), true);
		}

		public override bool CanFire(IDocumentWorkflow workflow, IStateMachineTrigger trigger)
		{
			using (Use(workflow))
				return trigger.CanFire();
		}

		public override async Task FireAsync(IDocumentWorkflow workflow, IStateMachineTrigger trigger, CancellationToken cancellationToken)
		{
			using (Use(workflow))
				await trigger.FireAsync(cancellationToken);
		}

		public override bool CanFire<TArg0>(IDocumentWorkflow workflow, IStateMachineTrigger<TArg0> trigger)
		{
			using (Use(workflow))
				return trigger.CanFire();
		}

		public override async Task FireAsync<TArg0>(IDocumentWorkflow workflow, IStateMachineTrigger<TArg0> trigger, TArg0 arg0,
			CancellationToken cancellationToken)
		{
			using (Use(workflow))
				await trigger.FireAsync(cancellationToken, arg0);
		}

		public override bool CanFire<TArg0, TArg1>(IDocumentWorkflow workflow, IStateMachineTrigger<TArg0, TArg1> trigger)
		{
			using (Use(workflow))
				return trigger.CanFire();
		}

		public override async Task FireAsync<TArg0, TArg1>(IDocumentWorkflow workflow, IStateMachineTrigger<TArg0, TArg1> trigger, TArg0 arg0, TArg1 arg1,
			CancellationToken cancellationToken)
		{
			using (Use(workflow))
				await trigger.FireAsync(cancellationToken, arg0, arg1);
		}

		public override bool CanFire<TArg0, TArg1, TArg2>(IDocumentWorkflow workflow, IStateMachineTrigger<TArg0, TArg1, TArg2> trigger)
		{
			using (Use(workflow))
				return trigger.CanFire();
		}

		public override async Task FireAsync<TArg0, TArg1, TArg2>(IDocumentWorkflow workflow, IStateMachineTrigger<TArg0, TArg1, TArg2> trigger, TArg0 arg0,
			TArg1 arg1, TArg2 arg2, CancellationToken cancellationToken)
		{
			using (Use(workflow))
				await trigger.FireAsync(cancellationToken, arg0, arg1, arg2);
		}


		private StateMachine<TDocumentState, TDocumentTrigger> CreateStateMachine()
		{
			var stateMachine = new StateMachine<TDocumentState, TDocumentTrigger>(GetState, SetState);
			Configure(new StateMachineConfiguration(this, stateMachine));
			return stateMachine;
		}

		protected abstract TDocumentState GetState();

		protected abstract void SetState(TDocumentState state);

		protected abstract void Configure(StateMachineConfiguration stateMachine);

		protected async Task OnEntryAsync(StateMachine<TDocumentState, TDocumentTrigger>.Transition transition)
		{
			if (transition.IsReentry)
			{
				string sourceState = transition.Source.ToString();
				await StrategyFactory.Perform(Current.GetType(), sourceState, strategy => strategy.OnReEnter(Current, null));
			}
			else
			{
				string destinationState = transition.Destination.ToString();
				await StrategyFactory.Perform(Current.GetType(), destinationState, async strategy =>
					{
						await strategy.OnEnter(Current, null);
					});
			}
		}

		protected async Task OnEntryFromAsync(StateMachine<TDocumentState, TDocumentTrigger>.Transition transition)
		{
			if (transition.IsReentry)
			{
				string sourceState = transition.Source.ToString();
				await StrategyFactory.Perform(Current.GetType(), sourceState, strategy => strategy.OnReEnter(Current, null));
			}
			else
			{
				string destinationState = transition.Destination.ToString();
				await StrategyFactory.Perform(Current.GetType(), destinationState, strategy => strategy.OnEnter(Current, null));
			}
		}

		protected async Task OnEntryFromAsync<TArg0>(TArg0 arg0, StateMachine<TDocumentState, TDocumentTrigger>.Transition transition)
		{
			if (transition.IsReentry)
			{
				string sourceState = transition.Source.ToString();
				await StrategyFactory.Perform(Current.GetType(), sourceState, strategy => strategy.OnReEnter(Current, new { arg0 }));
			}
			else
			{
				string destinationState = transition.Destination.ToString();
				await StrategyFactory.Perform(Current.GetType(), destinationState, strategy => strategy.OnEnter(Current, new { arg0 }));
			}
		}

		protected async Task OnEntryFromAsync<TArg0, TArg1>(TArg0 arg0, TArg1 arg1, StateMachine<TDocumentState, TDocumentTrigger>.Transition transition)
		{
			if (transition.IsReentry)
			{
				string sourceState = transition.Source.ToString();
				await StrategyFactory.Perform(Current.GetType(), sourceState, strategy => strategy.OnReEnter(Current, new { arg0, arg1 }));
			}
			else
			{
				string destinationState = transition.Destination.ToString();
				await StrategyFactory.Perform(Current.GetType(), destinationState, strategy => strategy.OnEnter(Current, new { arg0, arg1 }));
			}
		}

		protected async Task OnEntryFromAsync<TArg0, TArg1, TArg2>(TArg0 arg0, TArg1 arg1, TArg2 arg2,
			StateMachine<TDocumentState, TDocumentTrigger>.Transition transition)
		{
			if (transition.IsReentry)
			{
				string sourceState = transition.Source.ToString();
				await StrategyFactory.Perform(Current.GetType(), sourceState, strategy => strategy.OnReEnter(Current, new { arg0, arg1, arg2 }));
			}
			else
			{
				string destinationState = transition.Destination.ToString();
				await StrategyFactory.Perform(Current.GetType(), destinationState, strategy => strategy.OnEnter(Current, new { arg0, arg1, arg2 }));
			}
		}

		protected async Task OnExitAsync(StateMachine<TDocumentState, TDocumentTrigger>.Transition transition)
		{
			if (!transition.IsReentry)
			{
				string sourceState = transition.Source.ToString();
				await StrategyFactory.Perform(Current.GetType(), sourceState, strategy => strategy.OnLeave(Current));
			}
		}
	}

	public abstract class DocumentWorkflowStateMachineBase<TDocument, TKey, TDocumentState, TDocumentTrigger>
		: DocumentWorkflowStateMachineBase<TDocumentState, TDocumentTrigger>
		where TDocument: IDocumentEntity<TKey, TDocumentState>
		where TDocumentState: struct
		where TDocumentTrigger: struct
	{
		protected new IDocumentWorkflow<TDocument, TKey, TDocumentState> Current => (IDocumentWorkflow<TDocument, TKey, TDocumentState>) base.Current;

		private TDocument CurrentDocument => Current.Document;

		protected override TDocumentState GetState()
		{
			return CurrentDocument.State;
		}

		protected override void SetState(TDocumentState state)
		{
			AsyncHelper.RunSync(async () => await SaveState(state));
		}

		protected virtual async Task SaveState(TDocumentState state)
		{
			await Current.SaveState(state);
		}
	}
}
