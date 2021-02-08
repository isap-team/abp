using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Logging;

namespace Isap.CommonCore.API
{
	public interface IApiClient
	{
		ILogger Logger { get; set; }

		ApiRequestContext CreateRequestContext(CancellationToken cancellationToken = default(CancellationToken));

		ApiRequestContext CreateRequestContext(string apiSubdomain, CancellationToken cancellationToken = default(CancellationToken));

		ApiRequestContext CreateRequestContext(string apiLogin, string apiPassword,
			CancellationToken cancellationToken = default(CancellationToken));

		ApiRequestContext CreateRequestContext(string apiSubdomain, string apiLogin, string apiPassword,
			CancellationToken cancellationToken = default(CancellationToken));

		Task<AuthTokenItem> GetAuthToken(ApiRequestContext context);
	}
}
