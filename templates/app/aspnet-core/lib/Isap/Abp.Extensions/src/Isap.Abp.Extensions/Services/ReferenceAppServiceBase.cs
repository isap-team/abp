using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Isap.Abp.Extensions.DataFilters;
using Isap.Abp.Extensions.Domain;
using Isap.Abp.Extensions.Querying;
using Isap.CommonCore;
using Isap.CommonCore.Services;

namespace Isap.Abp.Extensions.Services
{
	public abstract class ReferenceAppServiceBase<TEntityDto, TIntf, TKey>: AppServiceBase<TEntityDto, TIntf>, IReferenceAppService<TEntityDto, TKey>
		where TEntityDto: class, ICommonEntityDto<TKey>
		where TIntf: class, ICommonEntity<TKey>
	{
		protected abstract string PermissionGroupName { get; }

		/// <summary>
		///     Наименование сущности, для которой реализован этот менеджер.
		/// </summary>
		protected virtual string DefaultPermissionName => $"{PermissionGroupName}.{typeof(TIntf).Name.Substring(1)}";

		/// <summary>
		///     Имя разрешения для проверки доступа на выборку данных из БД.
		/// </summary>
		protected virtual string QueryPermissionName => DefaultPermissionName;

		public abstract Task<TEntityDto> Get(TKey id);

		public abstract Task<Dictionary<TKey, TEntityDto>> GetMany(TKey[] idList);

		public abstract Task<ResultSet<TEntityDto>> GetPage(int pageNumber, int pageSize, bool countTotal = false, QueryOptionsDto queryOptions = null);

		public abstract Task<ResultSet<TEntityDto>> GetPage(int pageNumber, int pageSize, bool countTotal = false,
			List<SpecificationParameters> specifications = null);

		Task<ResultSet<TEntityDto>> IReferenceAppService<TEntityDto, TKey>.GetPage(int pageNumber, int pageSize, bool countTotal,
			ICollection<DataFilterValueDto> filterValues)
		{
			return GetPage(pageNumber, pageSize, countTotal, new QueryOptionsDto { FilterValues = filterValues });
		}

		Task<ResultSet<TEntityDto>> IReferenceAppService<TEntityDto, TKey>.QueryPage(int pageNumber, int pageSize, bool countTotal,
			QueryOptionsDto queryOptions)
		{
			return GetPage(pageNumber, pageSize, countTotal, queryOptions);
		}

		/// <summary>
		///     Проверяет доступ текущего пользователя на выборку данных.
		/// </summary>
		/// <returns></returns>
		// [AbpAllowAnonymous]
		protected virtual async Task CheckQueryPermission()
		{
			await CheckPermission(QueryPermissionName);
		}
	}

	public abstract class ReferenceAppServiceBase<TEntityDto, TIntf, TKey, TDataStore>: ReferenceAppServiceBase<TEntityDto, TIntf, TKey>
		where TEntityDto: class, ICommonEntityDto<TKey>
		where TIntf: class, ICommonEntity<TKey>
		where TDataStore: class, IReferenceDataStore<TIntf, TKey>
	{
		public TDataStore DataStore => LazyGetRequiredService<TDataStore>();

		public override async Task<TEntityDto> Get(TKey id)
		{
			await CheckQueryPermission();
			TIntf entry = await DataStore.Get(id);
			return ToDto(entry);
		}

		public override async Task<Dictionary<TKey, TEntityDto>> GetMany(TKey[] idList)
		{
			await CheckQueryPermission();
			if (idList == null)
				return null;
			List<TIntf> entries = await DataStore.GetMany(idList);
			return entries.ToDictionary(entry => entry.Id, ToDto);
		}

		public override async Task<ResultSet<TEntityDto>> GetPage(int pageNumber, int pageSize, bool countTotal = false, QueryOptionsDto queryOptions = null)
		{
			await CheckQueryPermission();
			ICollection<DataFilterValue> dataFilterValues = null;
			ICollection<SortOption> sortOptions = null;
			if (queryOptions != null)
			{
				dataFilterValues = ToDataFilterValues(queryOptions.FilterValues);
				sortOptions = ToSortOptions(queryOptions.SortOptions);
			}

			ResultSet<TIntf> resultSet = await DataStore.GetPage(pageNumber, pageSize, dataFilterValues, sortOptions, countTotal);
			ResultSet<TEntityDto> result = resultSet.Convert(ToDto);
			return result;
		}
	}

	public abstract class ReferenceAppServiceBase<TEntityDto, TIntf, TImpl, TKey, TDataStore>: ReferenceAppServiceBase<TEntityDto, TIntf, TKey>
		where TEntityDto: class, ICommonEntityDto<TKey>
		where TIntf: class, ICommonEntity<TKey>
		where TImpl: class, TIntf
		where TDataStore: class, IReferenceDataStore<TIntf, TImpl, TKey>
	{
		public TDataStore DataStore => LazyGetRequiredService<TDataStore>();

		public override async Task<TEntityDto> Get(TKey id)
		{
			await CheckQueryPermission();
			TIntf entry = await DataStore.Get(id);
			return ToDto(entry);
		}

		public override async Task<Dictionary<TKey, TEntityDto>> GetMany(TKey[] idList)
		{
			await CheckQueryPermission();
			if (idList == null)
				return null;
			List<TIntf> entries = await DataStore.GetMany(idList);
			return entries.ToDictionary(entry => entry.Id, ToDto);
		}

		public override async Task<ResultSet<TEntityDto>> GetPage(int pageNumber, int pageSize, bool countTotal = false, QueryOptionsDto queryOptions = null)
		{
			await CheckQueryPermission();
			ICollection<DataFilterValue> dataFilterValues = null;
			ICollection<SortOption> sortOptions = null;
			if (queryOptions != null)
			{
				dataFilterValues = ToDataFilterValues(queryOptions.FilterValues);
				sortOptions = ToSortOptions(queryOptions.SortOptions);
			}

			ResultSet<TIntf> resultSet = await DataStore.GetPage(pageNumber, pageSize, dataFilterValues, sortOptions, countTotal);
			ResultSet<TEntityDto> result = resultSet.Convert(ToDto);
			return result;
		}

		public override async Task<ResultSet<TEntityDto>> GetPage(int pageNumber, int pageSize, bool countTotal = false,
			List<SpecificationParameters> specifications = null)
		{
			await CheckQueryPermission();

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

			ResultSet<TIntf> resultSet = await DataStore.GetPage(pageNumber, pageSize, specifications, countTotal);
			return resultSet.Convert(ToDto);
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
