using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Api.Clients;
using Isap.CommonCore;
using Isap.CommonCore.Services;

namespace Isap.Abp.Extensions.Api
{
	public abstract class ReferencesServiceProxyBase<TEntityDto, TKey, TApiApplication, TApiClient, TApiOptions, TApiManager>
		: ServiceProxyBase<TApiApplication, TApiClient, TApiOptions, TApiManager>, IReferenceAppService<TEntityDto, TKey>
		where TEntityDto: ICommonEntityDto<TKey>
		where TApiClient: IAbpApiClient
		where TApiOptions: AbpApiOptions, new()
		where TApiApplication: IAbpApiApplication<TApiClient, TApiOptions>
		where TApiManager: class, IReferencesApiManager<TEntityDto, TKey>
	{
		/*
		public new ICmsSession AbpSession
		{
			get => (ICmsSession) base.AbpSession;
			set => base.AbpSession = value;
		}
		*/

		// [HttpGet]
		// [Route("get")]
		public virtual async Task<TEntityDto> Get(TKey id)
		{
			TEntityDto entry = await RemoteCall(async session => await ApiManager.Get(session, id));
			if (entry != null)
				await ComplementResult(entry);
			return entry;
		}

		// [HttpGet]
		// [Route("many")]
		public virtual async Task<Dictionary<TKey, TEntityDto>> GetMany(TKey[] idList)
		{
			Dictionary<TKey, TEntityDto> map = await RemoteCall(session => ApiManager.GetMany(session, idList));
			await ComplementResult(map.Values);
			return map;
		}

		// [HttpPost]
		// [Route("page")]
		public virtual async Task<ResultSet<TEntityDto>> GetPage(int pageNumber, int pageSize, bool countTotal = false, QueryOptionsDto queryOptions = null)
		{
			return await ComplementResult(await RemoteCall(session => ApiManager.GetPage(session, pageNumber, pageSize, countTotal, queryOptions)));
		}

		public async Task<ResultSet<TEntityDto>> GetPage(int pageNumber, int pageSize, bool countTotal = false,
			List<SpecificationParameters> specifications = null)
		{
			return await ComplementResult(await RemoteCall(session => ApiManager.GetPage(session, pageNumber, pageSize, countTotal, specifications)));
		}

		Task<ResultSet<TEntityDto>> IReferenceAppService<TEntityDto, TKey>.GetPage(int pageNumber, int pageSize, bool countTotal,
			ICollection<DataFilterValueDto> filterValues)
		{
			throw new NotSupportedException();
		}

#pragma warning disable 1066
		Task<ResultSet<TEntityDto>> IReferenceAppService<TEntityDto, TKey>.QueryPage(int pageNumber, int pageSize, bool countTotal = false,
			QueryOptionsDto queryOptions = null)
#pragma warning restore 1066
		{
			throw new NotSupportedException();
		}

		protected virtual async Task<TResultSet> ComplementResult<TResultSet>(TResultSet resultSet)
			where TResultSet: ResultSet<TEntityDto>
		{
			foreach (TEntityDto entry in resultSet.Data)
				await ComplementResult(entry);
			return resultSet;
		}

		protected virtual async Task ComplementResult(ICollection<TEntityDto> entries)
		{
			foreach (TEntityDto entry in entries)
				await ComplementResult(entry);
		}

		protected virtual Task<TEntityDto> ComplementResult(TEntityDto entry)
		{
			return Task.FromResult(entry);
		}
	}
}
