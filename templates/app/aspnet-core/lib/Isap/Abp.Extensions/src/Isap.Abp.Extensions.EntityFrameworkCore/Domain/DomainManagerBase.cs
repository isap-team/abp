using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Data;
using Isap.Abp.Extensions.Expressions;
using Isap.Abp.Extensions.Locks;
using Isap.CommonCore.DependencyInjection;
using Isap.CommonCore.Extensions;
using Isap.CommonCore.Services;
using Isap.Converters.Extensions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using Volo.Abp.Validation;

namespace Isap.Abp.Extensions.Domain
{
	/// <summary>
	///     Класс реализует базовую финкциональность менеджера по работе с сущностью с типом <see cref="TImpl" />, реализующим
	///     интерфейс <see cref="TIntf" />.
	/// </summary>
	/// <typeparam name="TIntf">Интерфейс сущности.</typeparam>
	/// <typeparam name="TImpl">Класс сущности, реализующий интерфейс <see cref="TIntf" />.</typeparam>
	/// <typeparam name="TKey">Тип значений первичного ключа.</typeparam>
	public abstract class DomainManagerBase<TIntf, TImpl, TKey>: ReferenceDataStoreBase<TIntf, TImpl, TKey>, IDomainManager<TIntf, TImpl, TKey>
		where TIntf: class, ICommonEntity<TKey>
		where TImpl: class, IEntity<TKey>, TIntf
	{
		/// <summary>
		///     Провайдер блокировок для синхронизации доступа к экземплярам сущности.
		/// </summary>
		public ILockManagerProvider LockManagerProvider { get; set; }

		/// <summary>
		///     Менеджер блокировок для синхронизации доступа к экземплярам сущности.
		/// </summary>
		public IEntityLockManager<TKey> LockManager
		{
			get
			{
				ILockManagerProvider provider = LockManagerProvider;
				if (provider == null)
					throw new InvalidOperationException("LockManagerProvider is not assigned.");
				return provider.Get<TIntf, TKey>();
			}
		}

		IEntityLockManager IDomainManager.LockManager => LockManager;

		/// <summary>
		///     Метод создания независимой копии данных.
		/// </summary>
		/// <param name="entry">Данные для инициализации независимой копии.</param>
		/// <returns>Независимая копия данных.</returns>
		public abstract TImpl CreateNew(TIntf entry);

		/// <summary>
		///     Позволяет редактировать информацию о сущности.
		/// </summary>
		/// <param name="entry">Информация о сущности.</param>
		/// <param name="update">Функция изменения сущности.</param>
		/// <returns>Информация о сущности, скорректированная в процессе сохранения.</returns>
		public async Task<TIntf> Edit(TIntf entry, Action<TImpl> update)
		{
			if (entry == null) throw new ArgumentNullException(nameof(entry));
			if (update == null) throw new ArgumentNullException(nameof(update));
			TImpl editableEntry = await GetEditable(entry);
			update(editableEntry);
			return editableEntry;
		}

		/// <summary>
		///     Позволяет редактировать информацию о сущности.
		/// </summary>
		/// <param name="entry">Информация о сущности.</param>
		/// <param name="update">Функция изменения сущности.</param>
		/// <returns>Информация о сущности, скорректированная в процессе сохранения.</returns>
		public async Task<TIntf> Edit(TIntf entry, Func<TImpl, Task<TImpl>> update)
		{
			if (entry == null) throw new ArgumentNullException(nameof(entry));
			if (update == null) throw new ArgumentNullException(nameof(update));
			TImpl editableEntry = await GetEditable(entry);
			editableEntry = await update(editableEntry);
			return editableEntry;
		}

		/// <summary>
		///     Приводит интерфейс к классу. Если приведение невозможно, то создается независимая копия, доступная для дальнейшего
		///     редактирования.
		/// </summary>
		/// <param name="entry">Исходные данные.</param>
		/// <returns>Экземпляр класса, доступный для редактирования.</returns>
		public override TImpl AsEditable(TIntf entry)
		{
			return entry as TImpl ?? CreateNew(entry);
		}

		/// <summary>
		///     Сохраняет информацию в БД. Добавляет или обновляет, если запись с указанным идентификатором уже существует в БД.
		/// </summary>
		/// <param name="entry">Экземпляр данных для сохранения.</param>
		/// <returns>Информация о сохраненной записи, доступная только для чтения.</returns>
		public virtual async Task<TIntf> Save(TImpl entry)
		{
			if (entry == null) throw new ArgumentNullException(nameof(entry));
			var cts = new CancellationTokenSource(5000);
			using (await LockManager.GetWriterLockAsync(((IEntity<TKey>) entry).Id, cts.Token))
				return await SaveInternal(entry);
		}

		/// <summary>
		///     Сохраняет (добавляет/обновляет) информацию.
		/// </summary>
		/// <param name="entry">Информация о сущности.</param>
		/// <param name="update">Функция изменения сущности.</param>
		/// <returns>Информация о сущности, скорректированная в процессе сохранения.</returns>
		public async Task<TIntf> Save(TIntf entry, Action<TImpl> update)
		{
			if (entry == null) throw new ArgumentNullException(nameof(entry));
			if (update == null) throw new ArgumentNullException(nameof(update));
			var cts = new CancellationTokenSource(5000);
			using (await LockManager.GetWriterLockAsync(entry.Id, cts.Token))
			{
				TImpl result = AsEditable(entry).With(update);
				return await SaveInternal(result);
			}
		}

		/// <summary>
		///     Сохраняет (добавляет/обновляет) информацию.
		/// </summary>
		/// <param name="entry">Информация о сущности.</param>
		/// <param name="update">Функция изменения сущности.</param>
		/// <returns>Информация о сущности, скорректированная в процессе сохранения.</returns>
		public async Task<TIntf> Save(TIntf entry, Func<TImpl, Task<TImpl>> update)
		{
			if (entry == null) throw new ArgumentNullException(nameof(entry));
			if (update == null) throw new ArgumentNullException(nameof(update));
			var cts = new CancellationTokenSource(5000);
			using (await LockManager.GetWriterLockAsync(entry.Id, cts.Token))
			{
				TImpl result = await AsEditable(entry).As(update);
				return await SaveInternal(result);
			}
		}

		/// <summary>
		///     Сохраняет (добавляет/обновляет) информацию.
		/// </summary>
		/// <param name="id">Идентификатор обновляемой сущности.</param>
		/// <param name="update">Функция изменения сущности.</param>
		/// <returns>Информация о сущности, скорректированная в процессе сохранения.</returns>
		public async Task<TIntf> Save(TKey id, Action<TImpl> update)
		{
			if (id == null) throw new ArgumentNullException(nameof(id));
			if (update == null) throw new ArgumentNullException(nameof(update));
			var cts = new CancellationTokenSource(5000);
			using (await LockManager.GetWriterLockAsync(id, cts.Token))
			{
				TImpl result = (await GetEditable(id)).With(update);
				return await SaveInternal(result);
			}
		}

		/// <summary>
		///     Сохраняет (добавляет/обновляет) информацию.
		/// </summary>
		/// <param name="id">Идентификатор обновляемой сущности.</param>
		/// <param name="update">Функция изменения сущности.</param>
		/// <returns>Информация о сущности, скорректированная в процессе сохранения.</returns>
		public async Task<TIntf> Save(TKey id, Func<TImpl, Task<TImpl>> update)
		{
			if (id == null) throw new ArgumentNullException(nameof(id));
			if (update == null) throw new ArgumentNullException(nameof(update));
			var cts = new CancellationTokenSource(5000);
			using (await LockManager.GetWriterLockAsync(id, cts.Token))
			{
				TImpl result = await (await GetEditable(id)).As(update);
				return await SaveInternal(result);
			}
		}

		/// <summary>
		///     Удаляет запись из БД.
		/// </summary>
		/// <param name="id">Идентификатор удаляемой записи.</param>
		/// <returns></returns>
		public virtual async Task Delete(TKey id)
		{
			if (id == null) throw new ArgumentNullException(nameof(id));
			var cts = new CancellationTokenSource(5000);
			using (await LockManager.GetWriterLockAsync(id, cts.Token))
				await DeleteInternal(id);
		}

		/// <summary>
		///     Удаляет из БД информацию о сущности. Фактически записи не удаляется, а лишь отмечается как удаленные и, в
		///     большинстве случаев, игнорируется при последующих выборках из БД.
		/// </summary>
		/// <param name="predicate">Условие поиска записей для удаления.</param>
		/// <returns></returns>
		public virtual async Task Delete(Expression<Func<TImpl, bool>> predicate)
		{
			if (predicate == null) throw new ArgumentNullException(nameof(predicate));
			await DeleteInternal(predicate);
		}

		/// <summary>
		///     Восстанавливает в БД информацию о сущности. Фактически для записи сбрасывается отметка "IsDeleted" в значение
		///     false.
		/// </summary>
		/// <param name="id">Идентификатор записи.</param>
		/// <returns></returns>
		public virtual async Task<TIntf> Undelete(TKey id)
		{
			if (id == null) throw new ArgumentNullException(nameof(id));
			var cts = new CancellationTokenSource(5000);
			using (await LockManager.GetWriterLockAsync(id, cts.Token))
				return await UndeleteInternal(id);
		}

		public abstract TRelatedImpl AddRelatedEntry<TRelatedImpl>(Func<ICollection<TRelatedImpl>> getCollection, TRelatedImpl entry)
			where TRelatedImpl: class;

		/// <summary>
		///     Сохраняет информацию в БД. Добавляет или обновляет, если запись с указанным идентификатором уже существует в БД.
		/// </summary>
		/// <param name="entry">Экземпляр данных для сохранения.</param>
		/// <returns>Информация о сохраненной записи, доступная только для чтения.</returns>
		protected abstract Task<TImpl> SaveInternal(TImpl entry);

		/// <summary>
		///     Удаляет запись из БД.
		/// </summary>
		/// <param name="id">Идентификатор удаляемой записи.</param>
		/// <returns></returns>
		protected abstract Task DeleteInternal(TKey id);

		/// <summary>
		///     Удаляет из БД информацию о сущности. Фактически записи не удаляется, а лишь отмечается как удаленные и, в
		///     большинстве случаев, игнорируется при последующих выборках из БД.
		/// </summary>
		/// <param name="predicate">Условие поиска записей для удаления.</param>
		/// <returns></returns>
		protected abstract Task DeleteInternal(Expression<Func<TImpl, bool>> predicate);

		/// <summary>
		///     Восстанавливает запись в БД. IsDeleted сбрасывается в значение false.
		/// </summary>
		/// <param name="id">Идентификатор удаляемой записи.</param>
		/// <returns></returns>
		protected abstract Task<TIntf> UndeleteInternal(TKey id);

		/// <summary>
		///     Обновляет (добавляет/изменяет/удаляет) список связанных сущностей, только в случае когда свойство, содержащее этот
		///     список содрежит значение отличное от null.
		/// </summary>
		/// <typeparam name="TRelatedImpl"></typeparam>
		/// <typeparam name="TRelatedKey">Тип ключа для сопоставления.</typeparam>
		/// <param name="existingEntry">Существующая запись, содержащая свойство коллекции связанных сущностей.</param>
		/// <param name="entryToSave">Обновленная запись, содержащая свойство коллекции связанных сущностей.</param>
		/// <param name="collectionExpression">Выражение для доступа к свойству, содержащему коллекцию связанных сущностей.</param>
		/// <param name="getKey">Функция получения ключа из элемента списка.</param>
		/// <param name="update">Функция обновления элемента коллекции.</param>
		/// <returns></returns>
		protected async Task UpdateRelatedEntries<TRelatedImpl, TRelatedKey>(TImpl existingEntry, TImpl entryToSave,
			Expression<Func<TImpl, IEnumerable<TRelatedImpl>>> collectionExpression, Func<TRelatedImpl, TRelatedKey> getKey,
			Func<TRelatedImpl, TRelatedImpl, Task> update)
			where TRelatedImpl: class
		{
			Func<TImpl, IEnumerable<TRelatedImpl>> getRelatedEnumerable = collectionExpression.Compile();

			ICollection<TRelatedImpl> GetRelatedCollection(TImpl implementation) =>
				(ICollection<TRelatedImpl>) getRelatedEnumerable(implementation);

			ICollection<TRelatedImpl> collectionToSave = GetRelatedCollection(entryToSave);
			if (collectionToSave != null)
			{
				ICollection<TRelatedImpl> existingCollection = GetRelatedCollection(existingEntry);
				if (existingCollection == null)
					await EnsureCollectionLoadedAsync(existingEntry, collectionExpression);

				Debug.Assert(existingCollection != null);
				await UpdateRelatedEntries(existingCollection, collectionToSave, getKey, update);
			}
		}

		/// <summary>
		///     Загружает коллекцию связанных сущностей, если она еще до сих пор не загружена.
		/// </summary>
		/// <typeparam name="TRelatedImpl">Класс связанной сущности.</typeparam>
		/// <param name="existingEntry">Запись, содержащая свойство коллекции связанных сущностей.</param>
		/// <param name="collectionExpression">Выражение для доступа к свойству, содержащему коллекцию связанных сущностей.</param>
		/// <returns></returns>
		protected virtual Task EnsureCollectionLoadedAsync<TRelatedImpl>(TImpl existingEntry,
			Expression<Func<TImpl, IEnumerable<TRelatedImpl>>> collectionExpression)
			where TRelatedImpl: class
		{
			throw new NotSupportedException();
		}

		/// <summary>
		///     Обновляет (добавляет/изменяет/удаляет) список связанных сущностей, только в случае когда свойство, содержащее этот
		///     список содрежит значение отличное от null.
		/// </summary>
		/// <typeparam name="TRelatedImpl">Класс связанной сущности.</typeparam>
		/// <typeparam name="TRelatedKey">Тип ключа для сопоставления.</typeparam>
		/// <param name="existingCollection">
		///     Коллекция связанных сущностей, полученная из свойства существующей записи, извлеченной
		///     из БД.
		/// </param>
		/// <param name="collectionToSave">Коллекция для сохранения.</param>
		/// <param name="getKey">Функция получения ключа из элемента списка.</param>
		/// <param name="update">Функция обновления элемента коллекции.</param>
		/// <returns></returns>
		protected virtual async Task UpdateRelatedEntries<TRelatedImpl, TRelatedKey>(
			[NotNull] ICollection<TRelatedImpl> existingCollection,
			[NotNull] ICollection<TRelatedImpl> collectionToSave,
			[NotNull] Func<TRelatedImpl, TRelatedKey> getKey,
			[NotNull] Func<TRelatedImpl, TRelatedImpl, Task> update)
			where TRelatedImpl: class
		{
			if (existingCollection == null) throw new ArgumentNullException(nameof(existingCollection));
			if (collectionToSave == null) throw new ArgumentNullException(nameof(collectionToSave));
			if (getKey == null) throw new ArgumentNullException(nameof(getKey));
			if (update == null) throw new ArgumentNullException(nameof(update));

			List<Tuple<TRelatedKey, TRelatedImpl, TRelatedImpl>> tuples = existingCollection
				.FullOuterJoin(collectionToSave, getKey, getKey, Tuple.Create)
				.ToList();
			foreach (Tuple<TRelatedKey, TRelatedImpl, TRelatedImpl> tuple in tuples)
			{
				TRelatedImpl existingEntry = tuple.Item2;
				TRelatedImpl entryToSave = tuple.Item3;
				if (existingEntry == null)
				{
					existingCollection.Add(entryToSave);
				}
				else if (entryToSave == null)
				{
					if (existingEntry is ISoftDelete deletionAuditedEntry)
						deletionAuditedEntry.IsDeleted = true;
					else
						existingCollection.Remove(existingEntry);
				}
				else
				{
					await update(existingEntry, entryToSave);
					if (existingEntry is ISoftDelete deletionAuditedEntry)
						deletionAuditedEntry.IsDeleted = false;
				}
			}
		}

		/// <summary>
		///     Регистрирует <see cref="EntityFrameworkQueryableExtensions.Include{TEntity,TProperty}" /> выражение для связанного
		///     объекта.
		/// </summary>
		/// <typeparam name="TProperty">Тип свойства, содержащего связанный объект.</typeparam>
		/// <param name="expression">Выражение для доступа к свойству, содержащему связанный объект.</param>
		/// <returns>Реестр дочерних Include выражений.</returns>
		protected void RegisterPropertyInclude<TProperty>(Expression<Func<TImpl, TProperty>> expression)
			where TProperty: class, IEntity<Guid>
		{
			IncludeExpressionRegistry.RegisterPropertyInclude(expression);
		}

		/// <summary>
		///     Регистрирует <see cref="EntityFrameworkQueryableExtensions.Include{TEntity,TProperty}" /> выражение для связанного
		///     объекта.
		/// </summary>
		/// <typeparam name="TProperty">Тип свойства, содержащего связанный объект.</typeparam>
		/// <param name="expression">Выражение для доступа к свойству, содержащему связанный объект.</param>
		/// <param name="registerChildIncludes">Метод для регистрации дочерних Include выражений.</param>
		protected void RegisterPropertyInclude<TProperty>(Expression<Func<TImpl, TProperty>> expression,
			Action<IIncludeExpressionRegistry<TProperty>> registerChildIncludes)
			where TProperty: class, IEntity<Guid>
		{
			IncludeExpressionRegistry.RegisterPropertyInclude(expression, registerChildIncludes);
		}

		/// <summary>
		///     Регистрирует <see cref="EntityFrameworkQueryableExtensions.Include{TEntity,TProperty}" /> выражение для списка
		///     связанных объектов.
		/// </summary>
		/// <typeparam name="TPropertyValue">Тип свойства, содержащего список связанных объектов.</typeparam>
		/// <param name="expression">Выражение для доступа к свойству, содержащему список связанных объектов.</param>
		protected void RegisterCollectionInclude<TPropertyValue>(Expression<Func<TImpl, IEnumerable<TPropertyValue>>> expression)
			where TPropertyValue: class, IEntity<Guid>
		{
			IncludeExpressionRegistry.RegisterCollectionInclude(expression);
		}

		/// <summary>
		///     Регистрирует <see cref="EntityFrameworkQueryableExtensions.Include{TEntity,TProperty}" /> выражение для списка
		///     связанных объектов.
		/// </summary>
		/// <typeparam name="TPropertyValue">Тип свойства, содержащего список связанных объектов.</typeparam>
		/// <param name="expression">Выражение для доступа к свойству, содержащему список связанных объектов.</param>
		/// <param name="registerChildIncludes">Метод для регистрации дочерних Include выражений.</param>
		protected void RegisterCollectionInclude<TPropertyValue>(Expression<Func<TImpl, IEnumerable<TPropertyValue>>> expression,
			Action<IIncludeExpressionRegistry<TPropertyValue>> registerChildIncludes)
			where TPropertyValue: class, IEntity<Guid>
		{
			IncludeExpressionRegistry.RegisterCollectionInclude(expression, registerChildIncludes);
		}

		/// <summary>
		///     Регистрирует обработчик, который будет выполнен в случае успешного завершения транзакции.
		/// </summary>
		/// <param name="handle">Синхронный обработчик.</param>
		protected void OnCurrentUnitOfWorkCompleted(Action handle)
		{
			CurrentUnitOfWork.OnCompleted(() => Task.Factory.StartNew(handle));
		}

		/// <summary>
		///     Регистрирует обработчик, который будет выполнен в случае успешного завершения транзакции.
		/// </summary>
		/// <param name="handle">Асинхронный обработчик.</param>
		protected void OnCurrentUnitOfWorkCompleted(Func<Task> handle)
		{
			CurrentUnitOfWork.OnCompleted(handle);
		}
	}

	/// <summary>
	///     Класс реализует базовую финкциональность менеджера по работе с сущностью с типом <see cref="TImpl" />, реализующим
	///     интерфейс <see cref="TIntf" />.
	/// </summary>
	/// <typeparam name="TIntf">Интерфейс сущности.</typeparam>
	/// <typeparam name="TImpl">Класс сущности, реализующий интерфейс <see cref="TIntf" />.</typeparam>
	/// <typeparam name="TKey">Тип значений первичного ключа.</typeparam>
	/// <typeparam name="TDataRepository">Тип интерфейса репозитория для работы с БД.</typeparam>
	/// <typeparam name="TEntityBuilder">Тип, использующийся для создания нового экземпляра сущности.</typeparam>
	public abstract class DomainManagerBase<TIntf, TImpl, TKey, TDataRepository, TEntityBuilder>: DomainManagerBase<TIntf, TImpl, TKey>
		where TIntf: class, ICommonEntity<TKey>
		where TImpl: class, IAssignable<TKey, TIntf>, TIntf, IEntity<TKey> //, new()
		where TDataRepository: class, IRepository<TImpl, TKey>
		where TEntityBuilder: IEntityBuilder<TIntf, TImpl>
	{
		/// <summary>
		///     Репозиторий для доступа к БД.
		/// </summary>
		protected TDataRepository DataRepository => LazyGetRequiredService<TDataRepository>();

		[PropertyInject(IsOptional = false)]
		public IIdGenerator<TKey> IdGenerator { get; set; }

		public ICurrentUser CurrentUser { get; set; }

		public TEntityBuilder EntityBuilder { protected get; set; }

		/// <summary>
		///     Метод создания независимой копии данных.
		/// </summary>
		/// <param name="entry">Данные для инициализации независимой копии.</param>
		/// <returns>Независимая копия данных.</returns>
		public override TImpl CreateNew(TIntf entry)
		{
			var result = EntityBuilder.CreateNew(entry);
			result.Assign(entry);
			return result;
		}

		/// <summary>
		///     Сохраняет информацию в БД. Добавляет или обновляет, если запись с указанным идентификатором уже существует в БД.
		/// </summary>
		/// <param name="entry">Экземпляр данных для сохранения.</param>
		/// <returns>Информация о сохраненной записи, доступная только для чтения.</returns>
		protected override async Task<TImpl> SaveInternal(TImpl entry)
		{
			TImpl existingEntry;
			using (DataFilter.Disable<ISoftDelete>())
				existingEntry = ((IEntity<TKey>) entry).Id.IsDefaultValue()
					? await FindByUniqueKey(entry)
					: await GetEditable(((IEntity<TKey>) entry).Id);

			if (existingEntry == null)
			{
				existingEntry = await CreateInternal(entry);
			}
			else
			{
				if (existingEntry is ISoftDelete deletedEntry)
					deletedEntry.IsDeleted = false;
				existingEntry = await UpdateInternal(existingEntry, entry);
			}

			await UpdateRelatedEntriesInternal(existingEntry, entry);

			await CurrentUnitOfWork.SaveChangesAsync();

			await EnsureLoaded(existingEntry);

			return existingEntry;
		}

		private async Task<TImpl> FindByUniqueKey(TImpl entry)
		{
			Expression<Func<TImpl, bool>> predicate = CreateUniqueKeyPredicate(entry);
			if (predicate == null)
				return null;
			return await IncludeRelatedData(GetQuery()).Where(predicate).FirstOrDefaultAsync();
		}

		protected abstract Expression<Func<TImpl, bool>> CreateUniqueKeyPredicate(TImpl entry);

		/// <summary>
		///     Добавляет запись в БД.
		/// </summary>
		/// <param name="entry">Экземпляр данных для сохранения.</param>
		/// <returns>Информация о сохраненной записи, доступная только для чтения.</returns>
		protected virtual async Task<TImpl> CreateInternal(TImpl entry)
		{
			if (entry is ICommonMultiTenant<Guid?> multiTenantEntry)
			{
				multiTenantEntry.TenantId = CurrentTenant.Id;
			}

			if (entry is ICommonOwnedEntity<Guid?> ownedEntry)
			{
				ownedEntry.OwnerId = ownedEntry.OwnerId ?? CurrentUser.Id;
			}

			entry = await NormalizeInternal(entry);

			List<ValidationResult> validationResults = (await Task.WhenAll(Validate(entry).Concat(ValidateOnCreate(entry))))
				.Where(r => r != ValidationResult.Success)
				.ToList();

			if (validationResults.Any())
				throw new AbpValidationException(L["CouldNotValidateOnCreating{0}", typeof(TImpl).Name], validationResults);

			if (((IEntity<TKey>) entry).Id.IsDefaultValue())
			{
				EntityHelper.TrySetId(entry, () => IdGenerator.NextValue());
			}

			return await DataRepository.InsertAsync(entry);
		}

		/// <summary>
		///     Обновляет запись в БД.
		/// </summary>
		/// <param name="existingEntry">Экземпляр существующей записи, извлеченной из БД.</param>
		/// <param name="entry">Экземпляр данных для сохранения.</param>
		/// <returns>Информация о сохраненной записи, доступная только для чтения.</returns>
		protected virtual async Task<TImpl> UpdateInternal(TImpl existingEntry, TImpl entry)
		{
			entry = await NormalizeInternal(entry);

			List<ValidationResult> validationResults = (await Task.WhenAll(Validate(entry).Concat(ValidateOnUpdate(existingEntry, entry))))
				.Where(r => r != ValidationResult.Success)
				.ToList();
			if (validationResults.Any())
				throw new AbpValidationException(L["CouldNotValidateOnUpdating{0}", typeof(TImpl).Name], validationResults);
			if (!ReferenceEquals(existingEntry, entry))
				existingEntry.Assign(entry);

			return existingEntry;
		}

		/// <summary>
		///     Приводит формат добавляемых/изменяемых значений к нормальному виду.
		/// </summary>
		/// <param name="entry">Запись содержащая добавляемые/обновляемые данные.</param>
		/// <returns>Запись содержащая нормализованные добавляемые/обновляемые данные.</returns>
		protected virtual Task<TImpl> NormalizeInternal(TImpl entry)
		{
			return Task.FromResult(entry);
		}

		/// <summary>
		///     Выполняет валидацию добавляемой записи.
		/// </summary>
		/// <param name="entry">Экземпляр данных для сохранения.</param>
		/// <returns>Список результатов валидации.</returns>
		protected virtual IEnumerable<Task<ValidationResult>> Validate(TImpl entry)
		{
			return Enumerable.Empty<Task<ValidationResult>>();
		}

		/// <summary>
		///     Выполняет валидацию добавляемой записи при создании.
		/// </summary>
		/// <param name="entry">Экземпляр данных для сохранения.</param>
		/// <returns>Список результатов валидации.</returns>
		protected virtual IEnumerable<Task<ValidationResult>> ValidateOnCreate(TImpl entry)
		{
			return Enumerable.Empty<Task<ValidationResult>>();
		}

		/// <summary>
		///     Выполняет валидацию обновляемой записи при обновлении.
		/// </summary>
		/// <param name="existingEntry">Экземпляр существующей записи, извлеченной из БД.</param>
		/// <param name="entry">Экземпляр данных для сохранения.</param>
		/// <returns>Список результатов валидации.</returns>
		protected virtual IEnumerable<Task<ValidationResult>> ValidateOnUpdate(TImpl existingEntry, TImpl entry)
		{
			return Enumerable.Empty<Task<ValidationResult>>();
		}

		/// <summary>
		///     Обновляет все списки связанных сущностей.
		/// </summary>
		/// <param name="existingEntry">Экземпляр существующей записи, извлеченной из БД.</param>
		/// <param name="entry">Экземпляр данных для сохранения.</param>
		/// <returns></returns>
		protected virtual Task UpdateRelatedEntriesInternal(TImpl existingEntry, TImpl entry)
		{
			return Task.CompletedTask;
		}

		/// <summary>
		///     Удаляет запись из БД.
		/// </summary>
		/// <param name="id">Идентификатор удаляемой записи.</param>
		/// <returns></returns>
		protected override Task DeleteInternal(TKey id)
		{
			return DataRepository.DeleteAsync(id);
		}

		/// <summary>
		///     Удаляет из БД информацию о сущности. Фактически записи не удаляется, а лишь отмечается как удаленные и, в
		///     большинстве случаев, игнорируется при последующих выборках из БД.
		/// </summary>
		/// <param name="predicate">Условие поиска записей для удаления.</param>
		/// <returns></returns>
		protected override async Task DeleteInternal(Expression<Func<TImpl, bool>> predicate)
		{
			await DataRepository.DeleteAsync(predicate, true);
		}

		/// <summary>
		///     Восстанавливает запись в БД. IsDeleted сбрасывается в значение false.
		/// </summary>
		/// <param name="id">Идентификатор удаляемой записи.</param>
		/// <returns></returns>
		protected override async Task<TIntf> UndeleteInternal(TKey id)
		{
			if (!typeof(ISoftDelete).IsAssignableFrom(typeof(TImpl)))
				throw new NotSupportedException($"Entity '{typeof(TImpl)}' is not supports soft deletion.");

			using (DataFilter.Disable<ISoftDelete>())
			{
				TImpl entry = await DataRepository.FindAsync(id);
				if (entry == null)
					return null;
				((ISoftDelete) entry).IsDeleted = false;
				return await DataRepository.UpdateAsync(entry, true);
			}
		}

		/// <summary>
		///     Возвращает объект, необходимый для построения запроса выборки данных из БД.
		/// </summary>
		/// <returns>Объект, необходимый для построения запроса выборки данных из БД.</returns>
		protected override IQueryable<TImpl> GetQuery()
		{
			return DataRepository;
		}

		/// <summary>
		///     Загружает коллекцию связанных сущностей, если она еще до сих пор не загружена.
		/// </summary>
		/// <typeparam name="TRelatedImpl">Класс связанной сущности.</typeparam>
		/// <param name="existingEntry">Запись, содержащая свойство коллекции связанных сущностей.</param>
		/// <param name="collectionExpression">Выражение для доступа к свойству, содержащему коллекцию связанных сущностей.</param>
		/// <returns></returns>
		protected override Task EnsureCollectionLoadedAsync<TRelatedImpl>(TImpl existingEntry,
			Expression<Func<TImpl, IEnumerable<TRelatedImpl>>> collectionExpression)
		{
			return DataRepository.EnsureCollectionLoadedAsync(existingEntry, collectionExpression);
		}

		/// <summary>
		///     Создает результат валидации.
		/// </summary>
		/// <param name="formatMessage">Функция форматирования сообщения о результате валидации.</param>
		/// <param name="propertyNames">Список валидированных свойств.</param>
		/// <returns>Сформированный резальтат валидации для последующей обработки.</returns>
		protected Task<ValidationResult> CreateValidationResult(Func<string, string> formatMessage, params string[] propertyNames)
		{
			ValidationResult result = new ValidationResult(formatMessage(string.Join(", ", propertyNames)), propertyNames);
			return Task.FromResult(result);
		}

		/// <summary>
		///     Создает результат валидации свойства на предмет указания значения.
		/// </summary>
		/// <param name="propertyNames">Список валидированных свойств.</param>
		/// <returns>Сформированный резальтат валидации для последующей обработки.</returns>
		protected Task<ValidationResult> PropertiesShouldNotBeEmptyValidationResult(params string[] propertyNames)
		{
			return CreateValidationResult(names => L["PropertiesShouldNotBeEmpty", names], propertyNames);
		}

		public override TRelatedImpl AddRelatedEntry<TRelatedImpl>(Func<ICollection<TRelatedImpl>> getCollection, TRelatedImpl entry)
		{
			DbContext dbContext = GetDbContextForEntity<TRelatedImpl>();
			EntityEntry<TRelatedImpl> entityEntry = dbContext.Entry(entry);
			getCollection().Add(entry);
			entityEntry.State = EntityState.Added;
			return entry;
		}

		//public IAbpExtDbContextTypeMatcher DbContextTypeMatcher { get; set; }

		protected virtual DbContext GetDbContextForEntity<TRelatedImpl>()
			where TRelatedImpl: class
		{
			return DataRepository.GetDbContext();
			//Type dbContextType = DbContextTypeMatcher.GetConcreteType(typeof(AbpDbContext), typeof(TRelatedImpl));
			//return UnitOfWorkManager.Current.GetDbContext<AbpDbContext>(dbContextType);
		}

		protected virtual DbSet<TRelatedImpl> GetDbSet<TRelatedImpl>()
			where TRelatedImpl: class
		{
			DbContext dbContext = GetDbContextForEntity<TRelatedImpl>();
			return dbContext.Set<TRelatedImpl>();
		}
	}

	/// <summary>
	///     Класс реализует базовую финкциональность менеджера по работе с сущностью с типом <see cref="TImpl" />, реализующим
	///     интерфейс <see cref="TIntf" />.
	/// </summary>
	/// <typeparam name="TIntf">Интерфейс сущности.</typeparam>
	/// <typeparam name="TImpl">Класс сущности, реализующий интерфейс <see cref="TIntf" />.</typeparam>
	/// <typeparam name="TKey">Тип значений первичного ключа.</typeparam>
	/// <typeparam name="TDataRepository">Тип интерфейса репозитория для работы с БД.</typeparam>
	public abstract class DomainManagerBase<TIntf, TImpl, TKey, TDataRepository>
		: DomainManagerBase<TIntf, TImpl, TKey, TDataRepository, DefaultEntityBuilder<TIntf, TImpl>>
		where TIntf: class, ICommonEntity<TKey>
		where TImpl: class, IAssignable<TKey, TIntf>, TIntf, IEntity<TKey>, new()
		where TDataRepository: class, IRepository<TImpl, TKey>
	{
	}
}
