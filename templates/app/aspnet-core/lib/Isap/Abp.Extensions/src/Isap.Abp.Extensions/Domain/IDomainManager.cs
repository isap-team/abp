using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Locks;
using Isap.CommonCore.Services;
using JetBrains.Annotations;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Uow;

namespace Isap.Abp.Extensions.Domain
{
	public interface IDomainManager
	{
		IEntityLockManager LockManager { get; }
		IUnitOfWorkManager UnitOfWorkManager { get; }
	}

	public interface IDomainManager<TIntf, TImpl, in TKey>: IReferenceDataStore<TIntf, TImpl, TKey>, IDomainManager
		where TIntf: class, ICommonEntity<TKey>
		where TImpl: class, IEntity<TKey>, TIntf
	{
		new IEntityLockManager<TKey> LockManager { get; }

		/// <summary>
		///     Создает новую запись для редактирования и заполняет запись данными из шаблона.
		/// </summary>
		/// <param name="entry"></param>
		/// <returns></returns>
		TImpl CreateNew([NotNull] TIntf entry);

		/// <summary>
		///     Позволяет редактировать информацию о сущности.
		/// </summary>
		/// <param name="entry">Информация о сущности.</param>
		/// <param name="update">Функция изменения сущности.</param>
		/// <returns>Информация о сущности, скорректированная в процессе сохранения.</returns>
		Task<TIntf> Edit([NotNull] TIntf entry, [NotNull] Action<TImpl> update);

		/// <summary>
		///     Позволяет редактировать информацию о сущности.
		/// </summary>
		/// <param name="entry">Информация о сущности.</param>
		/// <param name="update">Функция изменения сущности.</param>
		/// <returns>Информация о сущности, скорректированная в процессе сохранения.</returns>
		Task<TIntf> Edit([NotNull] TIntf entry, [NotNull] Func<TImpl, Task<TImpl>> update);

		/// <summary>
		///     Сохраняет (добавляет/обновляет) информацию.
		/// </summary>
		/// <param name="entry">Информация о сущности.</param>
		/// <returns>Информация о сущности, скорректированная в процессе сохранения.</returns>
		Task<TIntf> Save([NotNull] TImpl entry);

		/// <summary>
		///     Сохраняет (добавляет/обновляет) информацию.
		/// </summary>
		/// <param name="entry">Информация о сущности.</param>
		/// <param name="update">Функция изменения сущности.</param>
		/// <returns>Информация о сущности, скорректированная в процессе сохранения.</returns>
		Task<TIntf> Save([NotNull] TIntf entry, [NotNull] Action<TImpl> update);

		/// <summary>
		///     Сохраняет (добавляет/обновляет) информацию.
		/// </summary>
		/// <param name="entry">Информация о сущности.</param>
		/// <param name="update">Функция изменения сущности.</param>
		/// <returns>Информация о сущности, скорректированная в процессе сохранения.</returns>
		Task<TIntf> Save([NotNull] TIntf entry, [NotNull] Func<TImpl, Task<TImpl>> update);

		/// <summary>
		///     Сохраняет (добавляет/обновляет) информацию.
		/// </summary>
		/// <param name="id">Идентификатор обновляемой сущности.</param>
		/// <param name="update">Функция изменения сущности.</param>
		/// <returns>Информация о сущности, скорректированная в процессе сохранения.</returns>
		Task<TIntf> Save([NotNull] TKey id, [NotNull] Action<TImpl> update);

		/// <summary>
		///     Сохраняет (добавляет/обновляет) информацию.
		/// </summary>
		/// <param name="id">Идентификатор обновляемой сущности.</param>
		/// <param name="update">Функция изменения сущности.</param>
		/// <returns>Информация о сущности, скорректированная в процессе сохранения.</returns>
		Task<TIntf> Save([NotNull] TKey id, [NotNull] Func<TImpl, Task<TImpl>> update);

		/// <summary>
		///     Удаляет из БД информацию о сущности. Фактически запись не удаляется, а лишь отмечается как удаленная и, в
		///     большинстве случаев, игнорируется при последующих выборках из БД.
		/// </summary>
		/// <param name="id">Идентификатор записи.</param>
		/// <returns></returns>
		Task Delete([NotNull] TKey id);

		/// <summary>
		///     Удаляет из БД информацию о сущности. Фактически записи не удаляется, а лишь отмечается как удаленные и, в
		///     большинстве случаев, игнорируется при последующих выборках из БД.
		/// </summary>
		/// <param name="predicate">Условие поиска записей для удаления.</param>
		/// <returns></returns>
		Task Delete([NotNull] Expression<Func<TImpl, bool>> predicate);

		/// <summary>
		///     Восстанавливает в БД информацию о сущности. Фактически для записи сбрасывается отметка "IsDeleted" в значение
		///     false.
		/// </summary>
		/// <param name="id">Идентификатор записи.</param>
		/// <returns></returns>
		Task<TIntf> Undelete([NotNull] TKey id);

		/// <summary>
		///     Добавляет связанную сущность в коллекцию и включает отслеживание изменений для добавляемой сущности.
		/// </summary>
		/// <param name="getCollection">Делегат, возвращающий ссылку на коллекцию связанных сущностей.</param>
		/// <param name="entry">Сылка на добавляемую сущность.</param>
		/// <returns>Ссылка на добавленную сущность.</returns>
		Task<TRelatedImpl> AddRelatedEntry<TRelatedImpl>(Func<ICollection<TRelatedImpl>> getCollection, TRelatedImpl entry)
			where TRelatedImpl: class;
	}
}
