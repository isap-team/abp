using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Domain;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace Isap.Abp.FileStorage.Files
{
	public interface IFileDataManager: IDomainManager<IFileData, FileData, Guid>
	{
		Task<IFileData> IncreaseReferenceCount(Guid fileDataId);
		Task<IFileData> DecreaseReferenceCount(Guid fileDataId);
	}

	public class FileDataManager: DomainManagerBase<IFileData, FileData, Guid, IRepository<FileData, Guid>>, IFileDataManager
	{
		/// <summary>
		///     Увеличивает счетчик ссылок на указанный файл.
		/// </summary>
		/// <param name="fileDataId">Идентификатор файла</param>
		/// <returns>Вернет null если файл удален (isDeleted) или не найден.</returns>
		public async Task<IFileData> IncreaseReferenceCount(Guid fileDataId)
		{
			IFileData result;
			bool isDeleted = false;

			using (DataFilter.Disable<ISoftDelete>())
			{
				IFileData fileData = await Get(fileDataId);

				if (fileData == null)
					return null;

				if (fileData is ISoftDelete deletedEntry)
					isDeleted = deletedEntry.IsDeleted;

				result = await Save(fileData, e => e.ReferenceCount++);
			}

			if (isDeleted)
			{
				await Delete(fileDataId);
				return null;
			}

			return result;
		}

		/// <summary>
		///     Уменьшает счетчик ссылок на файл.
		/// </summary>
		/// <param name="fileDataId">Идентификатор файла</param>
		/// <returns>Вернет null если файл удален (isDeleted) или не найден.</returns>
		public async Task<IFileData> DecreaseReferenceCount(Guid fileDataId)
		{
			IFileData result;
			bool isDeleted = false;

			using (DataFilter.Disable<ISoftDelete>())
			{
				IFileData fileData = await GetEditable(fileDataId);

				if (fileData == null)
					return null;

				if (fileData is ISoftDelete deletedEntry)
					isDeleted = deletedEntry.IsDeleted;

				result = await Save(fileData, e => e.ReferenceCount--);
			}

			if (isDeleted)
			{
				await Delete(fileDataId);
				return null;
			}

			return result;
		}

		protected override IQueryable<FileData> DefaultSortQuery(IQueryable<FileData> query)
		{
			return query
					.OrderBy(e => e.ContentType)
					.ThenByDescending(e => e.CreationTime)
					.ThenByDescending(e => e.Id)
				;
		}

		protected override Expression<Func<FileData, bool>> CreateUniqueKeyPredicate(FileData entry)
		{
			return null;
		}
	}
}
