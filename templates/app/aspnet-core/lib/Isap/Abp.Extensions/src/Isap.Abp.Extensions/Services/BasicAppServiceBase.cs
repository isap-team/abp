using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Isap.Abp.Extensions.DataFilters;
using Isap.Abp.Extensions.Domain;
using Isap.Abp.Extensions.Querying;
using Isap.CommonCore;
using Isap.CommonCore.Services;
using Isap.Converters.Extensions;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Identity;

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
		protected TDomainManager DomainManager => LazyServiceProvider.LazyGetRequiredService<TDomainManager>();

		protected virtual string CreatePermissionName => $"{DefaultPermissionName}.Create";
		protected virtual string UpdatePermissionName => $"{DefaultPermissionName}.Update";
		protected virtual string UpdateOwnPermissionName => $"{UpdatePermissionName}.Own";
		protected virtual string UpdateOtherPermissionName => $"{UpdatePermissionName}.Other";
		protected virtual string DeletePermissionName => $"{DefaultPermissionName}.Delete";
		protected virtual string DeleteOwnPermissionName => $"{DeletePermissionName}.Own";
		protected virtual string DeleteOtherPermissionName => $"{DeletePermissionName}.Other";

		public override async Task<TEntityDto> Get(TKey id)
		{
			await CheckQueryPermission();
			TIntf entry = await DomainManager.Get(id);
			return ObjectMapper.Map<TIntf, TEntityDto>(entry);
		}

		public override async Task<Dictionary<TKey, TEntityDto>> GetMany(TKey[] idList)
		{
			await CheckQueryPermission();
			if (idList == null)
				return null;
			List<TIntf> entries = await DomainManager.GetMany(idList);
			return entries
				.ToDictionary(entry => entry.Id, entry => ObjectMapper.Map<TIntf, TEntityDto>(entry));
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

			ResultSet<TIntf> resultSet = await DomainManager.GetPage(pageNumber, pageSize, dataFilterValues, sortOptions, countTotal);
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

			ResultSet<TIntf> resultSet = await DomainManager.GetPage(pageNumber, pageSize, specifications, countTotal);
			return resultSet.Convert(ToDto);
		}

		public override async Task<TEntityDto> Create(TEntityDto entry)
		{
			await CheckCreatePermission(entry);
			TImpl mappedEntry = ObjectMapper.Map<TEntityDto, TImpl>(entry);
			TIntf savedEntry = await DomainManager.Save(mappedEntry);
			TEntityDto result = ToDto(savedEntry);
			return result;
		}

		public override async Task<TEntityDto> Update(TEntityDto entry)
		{
			await CheckUpdatePermission(entry);
			TImpl mappedEntry = ObjectMapper.Map<TEntityDto, TImpl>(entry);
			TIntf savedEntry = await DomainManager.Save(mappedEntry);
			TEntityDto result = ToDto(savedEntry);
			return result;
		}

		public override async Task Delete(TKey id)
		{
			TEntityDto entry = ObjectMapper.Map<TIntf, TEntityDto>(await DomainManager.Get(id));
			if (entry == null) return;

			await CheckDeletePermission(entry);
			await DomainManager.Delete(id);
		}

		public override async Task<TEntityDto> Undelete(TKey id)
		{
			TEntityDto entryDto = ObjectMapper.Map<TIntf, TEntityDto>(await DomainManager.Get(id));
			if (entryDto == null) return null;
			await CheckUpdatePermission(entryDto);
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

		protected virtual Task<ICollection<OrganizationUnit>> GetRelatedOrganizationUnits(TEntityDto entry) => Task.FromResult<ICollection<OrganizationUnit>>(null);

		/// <summary>
		///     Проверяет доступ текущего пользователя на добавление данных.
		/// </summary>
		/// <returns></returns>
		protected virtual async Task CheckCreatePermission(TEntityDto entry)
		{
			ICollection<OrganizationUnit> organizationUnits = await GetRelatedOrganizationUnits(entry);
			await CheckPermission(organizationUnits, CreatePermissionName);
		}

		/// <summary>
		///     Проверяет доступ текущего пользователя на изменение данных.
		/// </summary>
		/// <returns></returns>
		protected virtual async Task CheckUpdatePermission(TEntityDto entry)
		{
			ICollection<OrganizationUnit> organizationUnits = await GetRelatedOrganizationUnits(entry);
			await CheckPermission(organizationUnits, UpdatePermissionName);
			if (entry is ICommonOwnedEntity<Guid?> || typeof(ICommonOwnedEntity<Guid?>).IsAssignableFrom(typeof(TImpl)))
				await CheckOwnOrOtherPermission(entry, UpdateOwnPermissionName, UpdateOtherPermissionName);
		}

		/// <summary>
		///     Проверяет доступ текущего пользователя на удаление данных.
		/// </summary>
		/// <returns></returns>
		protected virtual async Task CheckDeletePermission(TEntityDto entry)
		{
			ICollection<OrganizationUnit> organizationUnits = await GetRelatedOrganizationUnits(entry);
			await CheckPermission(organizationUnits, DeletePermissionName);
			if (entry is ICommonOwnedEntity<Guid?> || typeof(ICommonOwnedEntity<Guid?>).IsAssignableFrom(typeof(TImpl)))
				await CheckOwnOrOtherPermission(entry, DeleteOwnPermissionName, DeleteOtherPermissionName);
		}

		protected async Task CheckOwnOrOtherPermission(TEntityDto entry, string ownPermissionName, string otherPermissionName)
		{
			ICollection<OrganizationUnit> organizationUnits = await GetRelatedOrganizationUnits(entry);
			Guid? ownerId = await GetOwnerId(entry);
			ownerId = ownerId ?? (entry.Id.IsDefaultValue() ? CurrentUser.Id : null);
			if (CurrentUser.Id == ownerId)
				await CheckPermission(organizationUnits, ownPermissionName);
			else
				await CheckPermission(organizationUnits, otherPermissionName);
		}

		/// <summary>
		///     Определяет владельца записи.
		/// </summary>
		/// <param name="entry">Запись для определения владельца.</param>
		/// <returns>Идентификатор владельца записи.</returns>
		protected virtual async Task<Guid?> GetOwnerId(TEntityDto entry)
		{
			TIntf dao = await DomainManager.Get(entry.Id);
			if (dao == null)
				return entry is ICommonOwnedEntity<Guid?> ownedEntity ? ownedEntity.OwnerId : null;

			return dao is ICommonOwnedEntity<Guid?> ownedDao ? ownedDao.OwnerId : null;
		}
	}
}
