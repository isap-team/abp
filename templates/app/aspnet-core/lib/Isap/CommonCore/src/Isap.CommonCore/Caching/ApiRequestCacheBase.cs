using System;
using System.Threading;
using System.Threading.Tasks;

namespace Isap.CommonCore.Caching
{
	public abstract class ApiRequestCacheBase: IApiRequestCache
	{
		public abstract TResult GetData<TParams, TResult>(string url, TParams @params, Func<TParams, TResult> getData, bool cacheResults = true);

		public virtual Task<TResult> GetDataAsync<TParams, TResult>(string url, TParams @params, Func<TParams, Task<TResult>> getData, bool cacheResults = true,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Factory.StartNew(() => GetData(url, @params, p => getData(p).Result, cacheResults), cancellationToken);
		}

		public abstract TResult PostData<TParams, TResult>(string url, TParams @params, Func<TParams, TResult> postData, bool cacheResults = true);

		public virtual Task<TResult> PostDataAsync<TParams, TResult>(string url, TParams @params, Func<TParams, Task<TResult>> postData,
			bool cacheResults = true,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Factory.StartNew(() => PostData(url, @params, p => postData(p).Result, cacheResults), cancellationToken);
		}

		public abstract TResult PutData<TParams, TResult>(string url, TParams @params, Func<TParams, TResult> putData, bool cacheResults = true);

		public virtual Task<TResult> PutDataAsync<TParams, TResult>(string url, TParams @params, Func<TParams, Task<TResult>> putData, bool cacheResults = true,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Factory.StartNew(() => PutData(url, @params, p => putData(p).Result, cacheResults), cancellationToken);
		}

		public abstract TResult DeleteData<TParams, TResult>(string url, TParams @params, Func<TParams, TResult> deleteData, bool cacheResults = true);

		public virtual Task<TResult> DeleteDataAsync<TParams, TResult>(string url, TParams @params, Func<TParams, Task<TResult>> deleteData,
			bool cacheResults = true,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Factory.StartNew(() => DeleteData(url, @params, p => deleteData(p).Result, cacheResults), cancellationToken);
		}
	}
}
