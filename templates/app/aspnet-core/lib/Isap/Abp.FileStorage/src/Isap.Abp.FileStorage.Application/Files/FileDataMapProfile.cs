using AutoMapper;
using Isap.Abp.Extensions;
using Volo.Abp.AutoMapper;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Users;

namespace Isap.Abp.FileStorage.Files
{
	public class FileDataMapProfile: Profile
	{
		public FileDataMapProfile()
		{
			CreateMap<IFileData, FileDataDto>()
				.ForMember(dto => dto.Url, opt => opt.MapFrom((src, dest, member, res) =>
					{
						var urlBuilder = (IUrlBuilder)res.Options.ServiceCtor(typeof(IUrlBuilder));
						return string.IsNullOrEmpty(src.Path) ? null : urlBuilder.BuildFileUrl(src.Path);
					}));

			CreateMap<FileDataDto, FileData>()
				.Ignore(e => e.ReferenceCount)
				.Ignore(e => e.IsArchive)
				.Ignore(e => e.ArchiveTime)
				.Ignore(e => e.Revision)
				.ForMember(e => e.TenantId, opt => opt.MapFrom((src, dest, member, res) => res.GetService<ICurrentTenant>()?.Id))
				.Ignore(e => e.IsDeleted)
				.Ignore(e => e.DeleterId)
				.Ignore(e => e.DeletionTime)
				.Ignore(e => e.LastModificationTime)
				.Ignore(e => e.LastModifierId)
				.Ignore(e => e.CreationTime)
				.ForMember(e => e.CreatorId, opt => opt.MapFrom((src, dest, member, res) => res.GetService<ICurrentUser>()?.Id))
				.Ignore(e => e.ExtraProperties)
				.Ignore(e => e.ConcurrencyStamp)
				;
		}
	}
}
