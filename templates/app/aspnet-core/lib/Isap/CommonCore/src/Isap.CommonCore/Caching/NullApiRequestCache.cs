using System;
using System.Threading;
using System.Threading.Tasks;

namespace Isap.CommonCore.Caching
{
	public class NullApiRequestCache: IApiRequestCache
	{
		public TResult GetData<TParams, TResult>(string url, TParams @params, Func<TParams, TResult> getData, bool cacheResults = true)
		{
			return getData(@params);
		}

		public Task<TResult> GetDataAsync<TParams, TResult>(string url, TParams @params, Func<TParams, Task<TResult>> getData, bool cacheResults = true,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return getData(@params);
		}

		public TResult PostData<TParams, TResult>(string url, TParams @params, Func<TParams, TResult> postData, bool cacheResults = true)
		{
			return postData(@params);
		}

		public Task<TResult> PostDataAsync<TParams, TResult>(string url, TParams @params, Func<TParams, Task<TResult>> postData, bool cacheResults = true,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return postData(@params);
		}

		public TResult PutData<TParams, TResult>(string url, TParams @params, Func<TParams, TResult> putData, bool cacheResults = true)
		{
			return putData(@params);
		}

		public Task<TResult> PutDataAsync<TParams, TResult>(string url, TParams @params, Func<TParams, Task<TResult>> putData, bool cacheResults = true,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return putData(@params);
		}

		public TResult DeleteData<TParams, TResult>(string url, TParams @params, Func<TParams, TResult> deleteData, bool cacheResults = true)
		{
			return deleteData(@params);
		}

		public Task<TResult> DeleteDataAsync<TParams, TResult>(string url, TParams @params, Func<TParams, Task<TResult>> deleteData, bool cacheResults = true,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return deleteData(@params);
		}
	}
}
