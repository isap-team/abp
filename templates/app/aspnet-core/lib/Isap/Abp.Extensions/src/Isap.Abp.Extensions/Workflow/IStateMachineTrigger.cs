using System.Threading;
using System.Threading.Tasks;

namespace Isap.Abp.Extensions.Workflow
{
	public interface IStateMachineTrigger
	{
		bool CanFire();
		Task FireAsync(CancellationToken cancellationToken);
	}

	public interface IStateMachineTrigger<in TArg0>
	{
		bool CanFire();
		Task FireAsync(CancellationToken cancellationToken, TArg0 arg0);
	}

	public interface IStateMachineTrigger<in TArg0, in TArg1>
	{
		bool CanFire();
		Task FireAsync(CancellationToken cancellationToken, TArg0 arg0, TArg1 arg1);
	}

	public interface IStateMachineTrigger<in TArg0, in TArg1, in TArg2>
	{
		bool CanFire();
		Task FireAsync(CancellationToken cancellationToken, TArg0 arg0, TArg1 arg1, TArg2 arg2);
	}
}
