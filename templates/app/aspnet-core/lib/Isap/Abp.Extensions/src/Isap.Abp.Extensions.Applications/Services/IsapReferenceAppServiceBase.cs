using System.Collections.Generic;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Domain;
using Isap.CommonCore;
using Isap.CommonCore.Services;
using Microsoft.AspNetCore.Mvc;

namespace Isap.Abp.Extensions.Services
{
	public abstract class IsapReferenceAppServiceBase<TEntityDto, TIntf, TImpl, TKey, TDataStore>
		: ReferenceAppServiceBase<TEntityDto, TIntf, TImpl, TKey, TDataStore>
		where TEntityDto: class, ICommonEntityDto<TKey>
		where TIntf: class, ICommonEntity<TKey>
		where TImpl: class, TIntf
		where TDataStore: class, IReferenceDataStore<TIntf, TImpl, TKey>
	{
		[HttpGet]
		[Route("get")]
		public override Task<TEntityDto> Get(TKey id)
		{
			return base.Get(id);
		}

		[HttpGet]
		[Route("many")]
		public override Task<Dictionary<TKey, TEntityDto>> GetMany(TKey[] idList)
		{
			return base.GetMany(idList);
		}

		[HttpPost]
		[Route("page")]
		public override Task<ResultSet<TEntityDto>> GetPage(int pageNumber, int pageSize, bool countTotal = false, [FromBody] QueryOptionsDto queryOptions = null)
		{
			return base.GetPage(pageNumber, pageSize, countTotal, queryOptions);
		}

		[HttpPost]
		[Route("pageBySpecs")]
		public override Task<ResultSet<TEntityDto>> GetPage(int pageNumber, int pageSize, bool countTotal = false, List<SpecificationParameters> specifications = null)
		{
			return base.GetPage(pageNumber, pageSize, countTotal, specifications);
		}
	}
}
