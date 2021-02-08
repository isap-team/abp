using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Web.Models;
using Isap.Abp.Extensions.Api.Clients;
using Isap.CommonCore;
using Isap.CommonCore.Services;

namespace Isap.Abp.Extensions.Api
{
	public interface IReferencesApiManager<TEntityDto, TKey>: IApiManager
	{
		Task<AjaxResponse<TEntityDto>> Get(IAbpApiSession session, TKey id);
		Task<AjaxResponse<Dictionary<TKey, TEntityDto>>> GetMany(IAbpApiSession session, params TKey[] idList);

		Task<AjaxResponse<ResultSet<TEntityDto>>> GetPage(IAbpApiSession session, int pageNumber, int pageSize, bool countTotal = false,
			QueryOptionsDto queryOptions = null);

		Task<AjaxResponse<ResultSet<TEntityDto>>> GetPage(IAbpApiSession session, int pageNumber, int pageSize, bool countTotal = false,
			List<SpecificationParameters> specifications = null);
	}

	public abstract class ReferenceApiManager<TEntityDto, TKey, TApiClient>: ApiManagerBase<TApiClient>, IReferencesApiManager<TEntityDto, TKey>
		where TApiClient: IAbpApiClient
	{
		protected ReferenceApiManager(TApiClient client, string serviceBaseUrl)
			: base(client, serviceBaseUrl)
		{
		}

		public async Task<AjaxResponse<TEntityDto>> Get(IAbpApiSession session, TKey id)
		{
			return await ProcessGetAsync<object, TEntityDto>(session, $"Get?id={id}", null);
		}

		public async Task<AjaxResponse<Dictionary<TKey, TEntityDto>>> GetMany(IAbpApiSession session, params TKey[] idList)
		{
			var request = new GetManyInput<TKey>(idList);
			return await ProcessGetAsync<GetManyInput<TKey>, Dictionary<TKey, TEntityDto>>(session, $"GetMany", request);
		}

		public async Task<AjaxResponse<ResultSet<TEntityDto>>> GetPage(IAbpApiSession session, int pageNumber, int pageSize,
			bool countTotal = false, QueryOptionsDto queryOptions = null)
		{
			return await ProcessPostAsync<QueryOptionsDto, ResultSet<TEntityDto>>(session,
				$"QueryPage?pageNumber={pageNumber}&pageSize={pageSize}&countTotal={countTotal.ToString().ToLower()}",
				queryOptions ?? new QueryOptionsDto());
		}

		public async Task<AjaxResponse<ResultSet<TEntityDto>>> GetPage(IAbpApiSession session, int pageNumber, int pageSize, bool countTotal = false,
			List<SpecificationParameters> specifications = null)
		{
			return await ProcessPostAsync<List<SpecificationParameters>, ResultSet<TEntityDto>>(session,
				$"page2?pageNumber={pageNumber}&pageSize={pageSize}&countTotal={countTotal.ToString().ToLower()}",
				specifications);
		}
	}
}
