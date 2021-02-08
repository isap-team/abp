using System;
using Isap.Abp.Extensions.Data;
using Isap.Abp.Extensions.Domain;

namespace Isap.Abp.FileStorage.Files
{
	public interface IFileData: IMultiTenantEntity<Guid>, IHasRevision
	{
		long Length { get; }
		string ContentType { get; }
		string FileName { get; }
		string Name { get; }
		string Path { get; }
		string Hash { get; }
		int ReferenceCount { get; }

		/// <summary>
		/// Файл был перемещен в архив (бекап)
		/// </summary>
		bool IsArchive { get; }

		/// <summary>
		/// Время события
		/// </summary>
		DateTime? ArchiveTime { get; }
	}

	public class FileData: MultiTenantFullAuditedAggregateRoot<Guid, IFileData>, IFileData
	{
		public FileData()
		{
		}

		public FileData(Guid id)
			: base(id)
		{
		}

		public long Length { get; set; }
		public string ContentType { get; set; }
		public string FileName { get; set; }
		public string Name { get; set; }
		public string Path { get; set; }
		public string Hash { get; set; }
		public int ReferenceCount { get; set; }
		public bool IsArchive { get; set; }
		public DateTime? ArchiveTime { get; set; }
		public long Revision { get; set; }

		protected override void InternalAssign(IFileData source)
		{
			Length = source.Length;
			ContentType = source.ContentType;
			FileName = source.FileName;
			Name = source.Name;
			Path = source.Path;
			Hash = source.Hash;
			ReferenceCount = source.ReferenceCount;
			IsArchive = source.IsArchive;
			ArchiveTime = source.ArchiveTime;
			Revision = source.Revision;
		}
	}
}
