using System;
using System.Threading;
using System.Threading.Tasks;

namespace Isap.CommonCore.Caching
{
	public abstract class AsyncApiRequestCacheBase: IApiRequestCache
	{
		public virtual TResult GetData<TParams, TResult>(string url, TParams @params, Func<TParams, TResult> getData, bool cacheResults = true)
		{
			return GetDataAsync(url, @params, p => Task.FromResult(getData(p)), cacheResults).Result;
		}

		public abstract Task<TResult> GetDataAsync<TParams, TResult>(string url, TParams @params, Func<TParams, Task<TResult>> getData,
			bool cacheResults = true, CancellationToken cancellationToken = default(CancellationToken));

		public virtual TResult PostData<TParams, TResult>(string url, TParams @params, Func<TParams, TResult> postData, bool cacheResults = true)
		{
			return PostDataAsync(url, @params, p => Task.FromResult(postData(p)), cacheResults).Result;
		}

		public abstract Task<TResult> PostDataAsync<TParams, TResult>(string url, TParams @params, Func<TParams, Task<TResult>> postData,
			bool cacheResults = true, CancellationToken cancellationToken = default(CancellationToken));

		public virtual TResult PutData<TParams, TResult>(string url, TParams @params, Func<TParams, TResult> putData, bool cacheResults = true)
		{
			return PutDataAsync(url, @params, p => Task.FromResult(putData(p)), cacheResults).Result;
		}

		public abstract Task<TResult> PutDataAsync<TParams, TResult>(string url, TParams @params, Func<TParams, Task<TResult>> putData,
			bool cacheResults = true, CancellationToken cancellationToken = default(CancellationToken));

		public virtual TResult DeleteData<TParams, TResult>(string url, TParams @params, Func<TParams, TResult> deleteData, bool cacheResults = true)
		{
			return DeleteDataAsync(url, @params, p => Task.FromResult(deleteData(p)), cacheResults).Result;
		}

		public abstract Task<TResult> DeleteDataAsync<TParams, TResult>(string url, TParams @params, Func<TParams, Task<TResult>> deleteData,
			bool cacheResults = true, CancellationToken cancellationToken = default(CancellationToken));
	}
}
