using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Isap.CommonCore.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Isap.CommonCore.Caching
{
	public class BasicApiRequestCache: ApiRequestCacheBase, IEnumerable<KeyValuePair<string, object>>
	{
		private readonly ConcurrentDictionary<string, object> _cache = new ConcurrentDictionary<string, object>();
		private readonly HashAlgorithm _hashAlgorithm = MD5.Create();

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return _cache.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public override TResult GetData<TParams, TResult>(string url, TParams @params, Func<TParams, TResult> getData, bool cacheResults = true)
		{
			return SendData(url, @params, p => getData(@params), cacheResults);
		}

		public override TResult PostData<TParams, TResult>(string url, TParams @params, Func<TParams, TResult> postData, bool cacheResults = true)
		{
			return SendData(url, @params, postData, cacheResults);
		}

		public override TResult PutData<TParams, TResult>(string url, TParams @params, Func<TParams, TResult> putData, bool cacheResults = true)
		{
			return SendData(url, @params, p => putData(@params), cacheResults);
		}

		public override TResult DeleteData<TParams, TResult>(string url, TParams @params, Func<TParams, TResult> deleteData, bool cacheResults = true)
		{
			return SendData(url, @params, p => deleteData(@params), cacheResults);
		}

		private TResult SendData<TParams, TResult>(string url, TParams @params, Func<TParams, TResult> sendData, bool cacheResults)
		{
			string requestKey = GetRequestKey(url, @params);
			object response;
			if (cacheResults)
			{
				response = _cache.GetOrAdd(requestKey, p => sendData(@params));
			}
			else if (!_cache.TryGetValue(requestKey, out response))
			{
				response = sendData(@params);
			}

			switch (response)
			{
				case Exception e:
					throw e;
				case TResult result:
					return result;
				default:
					throw new InvalidOperationException(
						$"Unexpected response type '{response.GetType().FullName}' when expected type is '{typeof(TResult).FullName}'.");
			}
		}

		private string GetRequestKey<TParams>(string url, TParams @params)
		{
			Dictionary<string, object> paramsMap = @params.AsNameToObjectMap(deep: true, onAssignMember: state =>
				{
					state.Ignore = false;
				});
			string serializedParams = JToken.FromObject(paramsMap).ToString(Formatting.None);
			byte[] bytes = Encoding.UTF8.GetBytes(serializedParams);
			string hash = BitConverter.ToString(_hashAlgorithm.ComputeHash(bytes)).Replace("-", "");
			return $"{url}#{bytes.Length:X8}#{hash}";
		}

		public void Add(string paramsKey, object response)
		{
			_cache.AddOrUpdate(paramsKey, response, (key, currentValue) => response);
		}
	}
}
