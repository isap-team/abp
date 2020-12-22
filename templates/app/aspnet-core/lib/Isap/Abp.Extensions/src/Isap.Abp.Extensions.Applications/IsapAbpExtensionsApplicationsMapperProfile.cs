using System;
using System.Linq;
using AutoMapper;
using Isap.Abp.Extensions.Data.Specifications;
using Isap.Abp.Extensions.Domain;
using Isap.CommonCore.Services;
using Volo.Abp;
using Volo.Abp.AutoMapper;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Users;

namespace Isap.Abp.Extensions
{
	public class IsapAbpExtensionsApplicationsMapperProfile: Profile
	{
		public IsapAbpExtensionsApplicationsMapperProfile()
		{
			/* You can configure your AutoMapper mapping configuration here.
			 * Alternatively, you can split your mapping configurations
			 * into multiple profile classes for a better organization. */
			CreateBaseMapsForKey<Guid>();
			CreateBaseMapsForKey<string>();

			CreateMap<SpecificationMetadata, SpecificationMetadataDto>()
				.ForMember(e => e.Types,
					opt => opt.MapFrom(
						(src, dest, value) => src.Types.Select(t => t.Name).ToList()
					)
				)
				;
		}

		protected void CreateBaseMapsForKey<TKey>()
		{
			CreateMap<ICommonEntity<TKey>, CommonEntityDto<TKey>>();

			CreateMap<CommonEntityDto<TKey>, Entity<TKey>>();

			CreateMap<SoftDeleteEntityDto<TKey>, CommonAggregateRoot<TKey>>()
				.Ignore(e => e.CreationTime)
				.Ignore(e => e.CreatorId)
				.Ignore(e => e.LastModificationTime)
				.Ignore(e => e.LastModifierId)
				.Ignore(e => e.DeletionTime)
				.Ignore(e => e.DeleterId)
				.Ignore(e => e.ConcurrencyStamp)
				.Ignore(e => e.ExtraProperties)
				;

			CreateMap<SoftDeleteEntityDto<TKey>, MultiTenantAggregateRoot<TKey>>()
				.IncludeBase<SoftDeleteEntityDto<TKey>, CommonAggregateRoot<TKey>>()
				.ForMember(e => e.TenantId, opt => opt.MapFrom((src, dest, member, res) => res.GetService<ICurrentTenant>()?.Id))
				;

			CreateMap<SoftDeleteEntityDto<TKey>, SoftDeleteEntity<TKey>>()
				;
			CreateMap<ISoftDeleteEntity<TKey>, SoftDeleteEntityDto<TKey>>()
				;

			CreateMap<IMultiTenantEntity<TKey>, SoftDeleteEntityDto<TKey>>()
				.IncludeBase<ISoftDeleteEntity<TKey>, SoftDeleteEntityDto<TKey>>()
				;

			CreateMap<SoftDeleteEntityDto<TKey>, MultiTenantEntity<TKey>>()
				.IncludeBase<SoftDeleteEntityDto<TKey>, SoftDeleteEntity<TKey>>()
				.ForMember(e => e.TenantId, opt => opt.MapFrom((src, dest, member, res) => res.GetService<ICurrentTenant>()?.Id))
				;

			CreateMap<IDocumentEntity<TKey>, DocumentEntityDto<TKey>>()
				.ForMember(e => e.IsDeleted, opt => opt.MapFrom((src, dest) =>
					{
						if (src is ISoftDelete softDelete)
							return softDelete.IsDeleted;
						if (src is ICommonSoftDelete commonSoftDelete)
							return commonSoftDelete.IsDeleted;
						return false;
					}))
				.ForMember(e => e.LastModificationTime, opt => opt.MapFrom(src => src.LastModificationTime ?? src.CreationTime))
				;
			CreateMap<DocumentEntityDto<TKey>, DocumentEntity<TKey>>()
				.IncludeBase<SoftDeleteEntityDto<TKey>, MultiTenantAggregateRoot<TKey>>()
				.ForMember(e => e.OwnerId, opt => opt.MapFrom((src, dest, member, res) => res.GetService<ICurrentUser>()?.Id))
				;

			CreateMap<CommonEntityDto<TKey>, MultiTenantEntity<TKey>>()
				.Ignore(e => e.IsDeleted)
				.ForMember(e => e.TenantId, opt => opt.MapFrom((src, dest, member, res) => res.GetService<ICurrentTenant>()?.Id))
				;
		}
	}
}
