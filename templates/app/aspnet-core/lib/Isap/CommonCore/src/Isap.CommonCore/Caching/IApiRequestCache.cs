using System;
using System.Threading;
using System.Threading.Tasks;

namespace Isap.CommonCore.Caching
{
	public interface IApiRequestCache
	{
		TResult GetData<TParams, TResult>(string url, TParams @params, Func<TParams, TResult> getData, bool cacheResults = true);

		Task<TResult> GetDataAsync<TParams, TResult>(string url, TParams @params, Func<TParams, Task<TResult>> getData, bool cacheResults = true,
			CancellationToken cancellationToken = default(CancellationToken));

		TResult PostData<TParams, TResult>(string url, TParams @params, Func<TParams, TResult> postData, bool cacheResults = true);

		Task<TResult> PostDataAsync<TParams, TResult>(string url, TParams @params, Func<TParams, Task<TResult>> postData, bool cacheResults = true,
			CancellationToken cancellationToken = default(CancellationToken));

		TResult PutData<TParams, TResult>(string url, TParams @params, Func<TParams, TResult> putData, bool cacheResults = true);

		Task<TResult> PutDataAsync<TParams, TResult>(string url, TParams @params, Func<TParams, Task<TResult>> putData, bool cacheResults = true,
			CancellationToken cancellationToken = default(CancellationToken));

		TResult DeleteData<TParams, TResult>(string url, TParams @params, Func<TParams, TResult> deleteData, bool cacheResults = true);

		Task<TResult> DeleteDataAsync<TParams, TResult>(string url, TParams @params, Func<TParams, Task<TResult>> deleteData, bool cacheResults = true,
			CancellationToken cancellationToken = default(CancellationToken));
	}
}
