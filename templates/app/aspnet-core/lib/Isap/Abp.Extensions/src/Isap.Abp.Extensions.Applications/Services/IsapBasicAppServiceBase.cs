using System.Collections.Generic;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Domain;
using Isap.CommonCore;
using Isap.CommonCore.Services;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Domain.Entities;

namespace Isap.Abp.Extensions.Services
{
	public abstract class IsapBasicAppServiceBase<TEntityDto, TIntf, TImpl, TKey, TDomainManager>
		: BasicAppServiceBase<TEntityDto, TIntf, TImpl, TKey, TDomainManager>
		where TEntityDto: class, ICommonEntityDto<TKey>
		where TIntf: class, ICommonEntity<TKey>
		where TImpl: class, IEntity<TKey>, TIntf
		where TDomainManager: class, IDomainManager<TIntf, TImpl, TKey>
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
		public override Task<ResultSet<TEntityDto>> GetPage(int pageNumber, int pageSize, bool countTotal = false, QueryOptionsDto queryOptions = null)
		{
			return base.GetPage(pageNumber, pageSize, countTotal, queryOptions);
		}

		[HttpPost]
		[Route("pageBySpecs")]
		public override Task<ResultSet<TEntityDto>> GetPage(int pageNumber, int pageSize, bool countTotal = false, List<SpecificationParameters> specifications = null)
		{
			return base.GetPage(pageNumber, pageSize, countTotal, specifications);
		}

		[HttpPost]
		[Route("create")]
		public override Task<TEntityDto> Create(TEntityDto entry)
		{
			return base.Create(entry);
		}

		[HttpPut]
		[Route("update")]
		public override Task<TEntityDto> Update(TEntityDto entry)
		{
			return base.Update(entry);
		}

		[HttpPost]
		[Route("save")]
		public override Task<TEntityDto> Save(TEntityDto entry)
		{
			return base.Save(entry);
		}

		[HttpDelete]
		[Route("delete")]
		public override Task Delete(TKey id)
		{
			return base.Delete(id);
		}

		[HttpPost]
		[Route("undelete")]
		public override Task<TEntityDto> Undelete(TKey id)
		{
			return base.Undelete(id);
		}
	}
}
