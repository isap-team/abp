using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;

namespace Isap.Abp.BackgroundJobs
{
	/// <summary>
	///     Defines interface of a background job.
	/// </summary>
	public interface IExtendedAsyncBackgroundJob<in TArgs>: IAsyncBackgroundJob<TArgs>
	{
		/// <summary>
		///     Executes the job with the <see cref="args" />.
		/// </summary>
		/// <param name="args">Job arguments.</param>
		/// <param name="cancellationToken"></param>
		Task<object> ExecuteAsync(TArgs args, CancellationToken cancellationToken = default);
	}
}
