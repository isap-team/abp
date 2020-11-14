using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Data.Specifications;
using Isap.Abp.Extensions.Data.Specifications.FilterSpecs;
using Isap.Abp.Extensions.Data.Specifications.OrderSpecs;
using Isap.Abp.Extensions.DataFilters;
using Isap.Abp.Extensions.Domain;
using Isap.Abp.Extensions.Querying;
using Isap.CommonCore;
using Isap.CommonCore.Services;
using Isap.Converters.Extensions;
using Volo.Abp.Domain.Entities;

namespace Isap.Abp.Extensions.Services
{
	public abstract class BasicAppServiceBase<TEntityDto, TIntf, TKey>: ReferenceAppServiceBase<TEntityDto, TIntf, TKey>, IBasicAppService<TEntityDto, TKey>
		where TEntityDto: class, ICommonEntityDto<TKey>
		where TIntf: class, ICommonEntity<TKey>
	{
		public abstract Task<TEntityDto> Create(TEntityDto entry);

		public abstract Task<TEntityDto> Update(TEntityDto entry);

		public virtual async Task<TEntityDto> Save(TEntityDto entry)
		{
			return entry.Id.IsDefaultValue()
					? await Create(entry)
					: await Update(entry)
				;
		}

		public abstract Task Delete(TKey id);

		public abstract Task<TEntityDto> Undelete(TKey id);
	}

	public abstract class BasicAppServiceBase<TEntityDto, TIntf, TImpl, TKey, TDomainManager>: BasicAppServiceBase<TEntityDto, TIntf, TKey>
		where TEntityDto: class, ICommonEntityDto<TKey>
		where TIntf: class, ICommonEntity<TKey>
		where TImpl: class, IEntity<TKey>, TIntf
		where TDomainManager: class, IDomainManager<TIntf, TImpl, TKey>
	{
		public TDomainManager DomainManager => LazyGetRequiredService<TDomainManager>();

		public override async Task<TEntityDto> Get(TKey id)
		{
			//await CheckPermissions(async () => await CheckQueryPermission());
			TIntf entry = await DomainManager.Get(id);
			return ObjectMapper.Map<TIntf, TEntityDto>(entry);
		}

		public override async Task<Dictionary<TKey, TEntityDto>> GetMany(TKey[] idList)
		{
			//await CheckPermissions(async () => await CheckQueryPermission());
			if (idList == null)
				return null;
			List<TIntf> entries = await DomainManager.GetMany(idList);
			return entries
				.ToDictionary(entry => entry.Id, entry => ObjectMapper.Map<TIntf, TEntityDto>(entry));
		}

		public override async Task<ResultSet<TEntityDto>> GetPage(int pageNumber, int pageSize, bool countTotal = false, QueryOptionsDto queryOptions = null)
		{
			//await CheckPermissions(async () => await CheckQueryPermission());
			ICollection<DataFilterValue> dataFilterValues = null;
			ICollection<SortOption> sortOptions = null;
			if (queryOptions != null)
			{
				dataFilterValues = ToDataFilterValues(queryOptions.FilterValues);
				sortOptions = ToSortOptions(queryOptions.SortOptions);
			}

			ResultSet<TIntf> resultSet = await DomainManager.GetPage(pageNumber, pageSize, dataFilterValues, sortOptions, countTotal);
			ResultSet<TEntityDto> result = resultSet.Convert(ToDto);
			return result;
		}

		public override async Task<ResultSet<TEntityDto>> GetPage(int pageNumber, int pageSize, bool countTotal = false,
			List<SpecificationParameters> specifications = null)
		{
			//await CheckPermissions(async () => await CheckQueryPermission());

			List<SpecificationParameters> defaultSpecs = GetDefaultSpecifications();
			if ((defaultSpecs?.Count ?? 0) > 0)
			{
				if (specifications == null)
				{
					specifications = defaultSpecs;
				}
				else
				{
					specifications = new List<SpecificationParameters>(specifications);
					foreach (SpecificationParameters spec in defaultSpecs)
					{
						if (specifications.Any(s => s.SpecId == spec.SpecId)) continue;
						specifications.Add(spec);
					}
				}
			}

			ResultSet<TIntf> resultSet = await DomainManager.GetPage(pageNumber, pageSize, specifications, countTotal);
			return resultSet.Convert(ToDto);
		}

		public override async Task<TEntityDto> Create(TEntityDto entry)
		{
			// await CheckPermissions(async () =>
			// 	{
			// 		await CheckEditPermission(entry);
			// 		await CheckCreatePermission(entry);
			// 	});
			TImpl mappedEntry = ObjectMapper.Map<TEntityDto, TImpl>(entry);
			TIntf savedEntry = await DomainManager.Save(mappedEntry);
			TEntityDto result = ToDto(savedEntry);
			return result;
		}

		public override async Task<TEntityDto> Update(TEntityDto entry)
		{
			// await CheckPermissions(async () =>
			// 	{
			// 		await CheckEditPermission(entry);
			// 		await CheckUpdatePermission(entry);
			// 	});
			TImpl mappedEntry = ObjectMapper.Map<TEntityDto, TImpl>(entry);
			TIntf savedEntry = await DomainManager.Save(mappedEntry);
			TEntityDto result = ToDto(savedEntry);
			return result;
		}

		public override async Task Delete(TKey id)
		{
			TEntityDto entry = ObjectMapper.Map<TIntf, TEntityDto>(await DomainManager.Get(id));
			if (entry == null) return;

			// await CheckPermissions(async () =>
			// 	{
			// 		await CheckEditPermission(entry);
			// 		await CheckDeletePermission(entry);
			// 	});
			await DomainManager.Delete(id);
		}

		public override async Task<TEntityDto> Undelete(TKey id)
		{
			TIntf entry = await DomainManager.Undelete(id);
			return ToDto(entry);
		}

		protected virtual List<SpecificationParameters> GetDefaultSpecifications()
		{
			return null;
		}

		protected virtual TImpl ToDomain(TEntityDto entry)
		{
			return ObjectMapper.Map<TEntityDto, TImpl>(entry);
		}
	}
}
