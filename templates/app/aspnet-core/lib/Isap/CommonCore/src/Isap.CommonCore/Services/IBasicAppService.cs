using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Isap.CommonCore.Services
{
	public interface IBasicAppService
	{
	}

	public interface IBasicAppService<TEntityDto, TKey>: IReferenceAppService<TEntityDto, TKey>, IBasicAppService
		where TEntityDto: ICommonEntityDto<TKey>
	{
		/// <summary>
		///     Добавляет информацию об элементе сущности.
		/// </summary>
		/// <param name="entry">Информация для сохранения.</param>
		/// <returns>Информация, скорректированная в процессе сохранения.</returns>
		Task<TEntityDto> Create([NotNull] TEntityDto entry);

		/// <summary>
		///     Обновляет информацию об элементе сущности.
		/// </summary>
		/// <param name="entry">Информация для сохранения.</param>
		/// <returns>Информация, скорректированная в процессе сохранения.</returns>
		Task<TEntityDto> Update([NotNull] TEntityDto entry);

		/// <summary>
		///     Сохраняет (добавляет или обновляет) информацию об элементе сущности.
		/// </summary>
		/// <param name="entry">Информация для сохранения.</param>
		/// <returns>Информация, скорректированная в процессе сохранения.</returns>
		Task<TEntityDto> Save([NotNull] TEntityDto entry);

		/// <summary>
		///     Удаляет из БД информацию об элементе сущности. Фактически запись не удаляется, а лишь отмечается как удаленная и,
		///     в большинстве случаев, игнорируется при последующих выборках из БД.
		/// </summary>
		/// <param name="id">Идентификатор записи.</param>
		/// <returns></returns>
		Task Delete(TKey id);

		/// <summary>
		///     Восстанавливает в БД информацию о сущности. Фактически для записи сбрасывается отметка "IsDeleted" в значение
		///     false.
		/// </summary>
		/// <param name="id">Идентификатор записи.</param>
		/// <returns></returns>
		Task<TEntityDto> Undelete(TKey id);
	}

	public interface IBasicAppService<TEntityDto>: IBasicAppService<TEntityDto, Guid>
		where TEntityDto: ICommonEntityDto<Guid>
	{
	}
}
