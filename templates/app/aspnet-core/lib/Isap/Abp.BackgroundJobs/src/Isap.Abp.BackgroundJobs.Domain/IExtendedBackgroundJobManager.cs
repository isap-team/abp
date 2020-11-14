using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;

namespace Isap.Abp.BackgroundJobs
{
	public interface IExtendedBackgroundJobManager: IBackgroundJobManager
	{
		/// <summary>Enqueues a job to be executed.</summary>
		/// <typeparam name="TArgs">Type of the arguments of job.</typeparam>
		/// <param name="args">Job arguments.</param>
		/// <param name="cancellationToken">Cancellation token to cancel enqueue operation.</param>
		/// <param name="priority">Job priority.</param>
		/// <param name="delay">Job delay (wait duration before first try).</param>
		/// <returns>Unique identifier of a background job.</returns>
		Task<string> EnqueueAsync<TArgs>(TArgs args, CancellationToken cancellationToken,
			BackgroundJobPriority priority = BackgroundJobPriority.Normal, TimeSpan? delay = null);

		/// <summary>Enqueues a job to be executed.</summary>
		/// <typeparam name="TArgs">Type of the arguments of job.</typeparam>
		/// <param name="args">Job arguments.</param>
		/// <param name="concurrencyKey">Concurrency key for sequence dequeue.</param>
		/// <param name="priority">Job priority.</param>
		/// <param name="delay">Job delay (wait duration before first try).</param>
		/// <param name="cancellationToken">Cancellation token to cancel enqueue operation.</param>
		/// <returns>Unique identifier of a background job.</returns>
		Task<string> EnqueueAsync<TArgs>(TArgs args, string concurrencyKey, BackgroundJobPriority priority = BackgroundJobPriority.Normal,
			TimeSpan? delay = null, CancellationToken cancellationToken = default);

		/// <summary>
		///     Добавляет в указанную очередь задачу отложенной обработки.
		/// </summary>
		/// <typeparam name="TArgs">Тип аргументов для задачи.</typeparam>
		/// <param name="queueName">Имя очереди, в которой осуществляется поиск.</param>
		/// <param name="args">Значения аргументов для поиска.</param>
		/// <param name="priority">Приоритет обработки задачи.</param>
		/// <param name="delay">Интервал ожидания перед запуском.</param>
		/// <param name="cancellationToken">Cancellation token to cancel enqueue operation.</param>
		/// <returns>Уникальный идентификатор задания.</returns>
		Task<string> EnqueueAsync<TArgs>(string queueName, TArgs args, BackgroundJobPriority priority = BackgroundJobPriority.Normal,
			TimeSpan? delay = null, CancellationToken cancellationToken = default);

		/// <summary>
		///     Добавляет в указанную очередь задачу отложенной обработки.
		/// </summary>
		/// <typeparam name="TArgs">Тип аргументов для задачи.</typeparam>
		/// <param name="queueName">Имя очереди, в которой осуществляется поиск.</param>
		/// <param name="args">Значения аргументов для поиска.</param>
		/// <param name="concurrencyKey">Ключ для последовательного вывода конкурируюих задач.</param>
		/// <param name="priority">Приоритет обработки задачи.</param>
		/// <param name="delay">Интервал ожидания перед запуском.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Уникальный идентификатор задания.</returns>
		Task<string> EnqueueAsync<TArgs>(string queueName, TArgs args, string concurrencyKey,
			BackgroundJobPriority priority = BackgroundJobPriority.Normal, TimeSpan? delay = null, CancellationToken cancellationToken = default);

		/// <summary>Enqueues a job to be executed.</summary>
		/// <typeparam name="TArgs">Type of the arguments of job.</typeparam>
		/// <param name="args">Job arguments.</param>
		/// <param name="nextTryTime">Next try time.</param>
		/// <param name="priority">Job priority.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Unique identifier of a background job.</returns>
		Task<string> EnqueueAsync<TArgs>(TArgs args, DateTime nextTryTime,
			BackgroundJobPriority priority = BackgroundJobPriority.Normal, CancellationToken cancellationToken = default);

		/// <summary>Enqueues a job to be executed.</summary>
		/// <typeparam name="TArgs">Type of the arguments of job.</typeparam>
		/// <param name="args">Job arguments.</param>
		/// <param name="concurrencyKey">Concurrency key for sequence dequeue.</param>
		/// <param name="nextTryTime">Next try time.</param>
		/// <param name="priority">Job priority.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Unique identifier of a background job.</returns>
		Task<string> EnqueueAsync<TArgs>(TArgs args, string concurrencyKey, DateTime nextTryTime,
			BackgroundJobPriority priority = BackgroundJobPriority.Normal, CancellationToken cancellationToken = default);

		/// <summary>
		///     Добавляет в указанную очередь задачу отложенной обработки.
		/// </summary>
		/// <typeparam name="TArgs">Тип аргументов для задачи.</typeparam>
		/// <param name="queueName">Имя очереди, в которой осуществляется поиск.</param>
		/// <param name="args">Значения аргументов для поиска.</param>
		/// <param name="nextTryTime">Время запуска обработчика задачи.</param>
		/// <param name="priority">Приоритет обработки задачи.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Уникальный идентификатор задания.</returns>
		Task<string> EnqueueAsync<TArgs>(string queueName, TArgs args, DateTime nextTryTime,
			BackgroundJobPriority priority = BackgroundJobPriority.Normal, CancellationToken cancellationToken = default);

		/// <summary>
		///     Добавляет в указанную очередь задачу отложенной обработки.
		/// </summary>
		/// <typeparam name="TArgs">Тип аргументов для задачи.</typeparam>
		/// <param name="queueName">Имя очереди, в которой осуществляется поиск.</param>
		/// <param name="args">Значения аргументов для поиска.</param>
		/// <param name="concurrencyKey">Ключ для последовательного вывода конкурируюих задач.</param>
		/// <param name="nextTryTime">Время запуска обработчика задачи.</param>
		/// <param name="priority">Приоритет обработки задачи.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Уникальный идентификатор задания.</returns>
		Task<string> EnqueueAsync<TArgs>(string queueName, TArgs args, string concurrencyKey, DateTime nextTryTime,
			BackgroundJobPriority priority = BackgroundJobPriority.Normal, CancellationToken cancellationToken = default);

		/// <summary>
		///     Поиск задачи отложенной обработки по набору параметров в очереди, назначенной посредством атрибута
		///     <see cref="BackgroundJobAttribute" />, или в очереди по умолчанию.
		/// </summary>
		/// <typeparam name="TArgs">Тип аргументов для задачи.</typeparam>
		/// <param name="args">Значения аргументов для поиска.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Уникальный идентификатор задания.</returns>
		Task<string> FindAsync<TArgs>(TArgs args, CancellationToken cancellationToken = default);

		/// <summary>
		///     Поиск задачи отложенной обработки по имени очереди и набору параметров.
		/// </summary>
		/// <typeparam name="TArgs">Тип аргументов для задачи.</typeparam>
		/// <param name="queueName">Имя очереди, в которой осуществляется поиск.</param>
		/// <param name="args">Значения аргументов для поиска.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Уникальный идентификатор задания.</returns>
		Task<string> FindAsync<TArgs>(string queueName, TArgs args, CancellationToken cancellationToken = default);

		/// <summary>
		///     Добавляет в назначенную посредством атрибута <see cref="BackgroundJobAttribute" /> очередь или в очереди по
		///     умолчанию
		///     задачу отложенной обработки, но только в случае если такой задачи еще нет в очереди.
		/// </summary>
		/// <typeparam name="TArgs">Тип аргументов для задачи.</typeparam>
		/// <param name="args">Значения аргументов для поиска.</param>
		/// <param name="priority">Приоритет обработки задачи.</param>
		/// <param name="delay">Интервал ожидания перед запуском.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Уникальный идентификатор задания.</returns>
		Task<string> EnsureAsync<TArgs>(TArgs args, BackgroundJobPriority priority = BackgroundJobPriority.Normal,
			TimeSpan? delay = null, CancellationToken cancellationToken = default);

		/// <summary>
		///     Добавляет в назначенную посредством атрибута <see cref="BackgroundJobAttribute" /> очередь или в очереди по
		///     умолчанию
		///     задачу отложенной обработки, но только в случае если такой задачи еще нет в очереди.
		/// </summary>
		/// <typeparam name="TArgs">Тип аргументов для задачи.</typeparam>
		/// <param name="args">Значения аргументов для поиска.</param>
		/// <param name="concurrencyKey">Ключ для последовательного вывода конкурируюих задач.</param>
		/// <param name="priority">Приоритет обработки задачи.</param>
		/// <param name="delay">Интервал ожидания перед запуском.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Уникальный идентификатор задания.</returns>
		Task<string> EnsureAsync<TArgs>(TArgs args, string concurrencyKey, BackgroundJobPriority priority = BackgroundJobPriority.Normal,
			TimeSpan? delay = null, CancellationToken cancellationToken = default);

		/// <summary>
		///     Добавляет в указанную очередь задачу отложенной обработки, но только в случае если такой задачи еще нет в очереди.
		/// </summary>
		/// <typeparam name="TArgs">Тип аргументов для задачи.</typeparam>
		/// <param name="queueName">Имя очереди, в которой осуществляется поиск.</param>
		/// <param name="args">Значения аргументов для поиска.</param>
		/// <param name="priority">Приоритет обработки задачи.</param>
		/// <param name="delay">Интервал ожидания перед запуском.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Уникальный идентификатор задания.</returns>
		Task<string> EnsureAsync<TArgs>(string queueName, TArgs args, BackgroundJobPriority priority = BackgroundJobPriority.Normal,
			TimeSpan? delay = null, CancellationToken cancellationToken = default);

		/// <summary>
		///     Добавляет в указанную очередь задачу отложенной обработки, но только в случае если такой задачи еще нет в очереди.
		/// </summary>
		/// <typeparam name="TArgs">Тип аргументов для задачи.</typeparam>
		/// <param name="queueName">Имя очереди, в которой осуществляется поиск.</param>
		/// <param name="args">Значения аргументов для поиска.</param>
		/// <param name="concurrencyKey">Ключ для последовательного вывода конкурируюих задач.</param>
		/// <param name="priority">Приоритет обработки задачи.</param>
		/// <param name="delay">Интервал ожидания перед запуском.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Уникальный идентификатор задания.</returns>
		Task<string> EnsureAsync<TArgs>(string queueName, TArgs args, string concurrencyKey,
			BackgroundJobPriority priority = BackgroundJobPriority.Normal, TimeSpan? delay = null, CancellationToken cancellationToken = default);

		/// <summary>
		///     Добавляет в назначенную посредством атрибута <see cref="BackgroundJobAttribute" /> очередь или в очереди по
		///     умолчанию
		///     задачу отложенной обработки, но только в случае если такой задачи еще нет в очереди.
		/// </summary>
		/// <typeparam name="TArgs">Тип аргументов для задачи.</typeparam>
		/// <param name="args">Значения аргументов для поиска.</param>
		/// <param name="nextTryTime">Время запуска обработчика задачи.</param>
		/// <param name="priority">Приоритет обработки задачи.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Уникальный идентификатор задания.</returns>
		Task<string> EnsureAsync<TArgs>(TArgs args, DateTime nextTryTime,
			BackgroundJobPriority priority = BackgroundJobPriority.Normal, CancellationToken cancellationToken = default);

		/// <summary>
		///     Добавляет в назначенную посредством атрибута <see cref="BackgroundJobAttribute" /> очередь или в очереди по
		///     умолчанию
		///     задачу отложенной обработки, но только в случае если такой задачи еще нет в очереди.
		/// </summary>
		/// <typeparam name="TArgs">Тип аргументов для задачи.</typeparam>
		/// <param name="args">Значения аргументов для поиска.</param>
		/// <param name="concurrencyKey">Ключ для последовательного вывода конкурируюих задач.</param>
		/// <param name="nextTryTime">Время запуска обработчика задачи.</param>
		/// <param name="priority">Приоритет обработки задачи.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Уникальный идентификатор задания.</returns>
		Task<string> EnsureAsync<TArgs>(TArgs args, string concurrencyKey, DateTime nextTryTime,
			BackgroundJobPriority priority = BackgroundJobPriority.Normal, CancellationToken cancellationToken = default);

		/// <summary>
		///     Добавляет в указанную очередь задачу отложенной обработки, но только в случае если такой задачи еще нет в очереди.
		/// </summary>
		/// <typeparam name="TArgs">Тип аргументов для задачи.</typeparam>
		/// <param name="queueName">Имя очереди, в которой осуществляется поиск.</param>
		/// <param name="args">Значения аргументов для поиска.</param>
		/// <param name="nextTryTime">Время запуска обработчика задачи.</param>
		/// <param name="priority">Приоритет обработки задачи.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Уникальный идентификатор задания.</returns>
		Task<string> EnsureAsync<TArgs>(string queueName, TArgs args, DateTime nextTryTime,
			BackgroundJobPriority priority = BackgroundJobPriority.Normal, CancellationToken cancellationToken = default);

		/// <summary>
		///     Добавляет в указанную очередь задачу отложенной обработки, но только в случае если такой задачи еще нет в очереди.
		/// </summary>
		/// <typeparam name="TArgs">Тип аргументов для задачи.</typeparam>
		/// <param name="queueName">Имя очереди, в которой осуществляется поиск.</param>
		/// <param name="args">Значения аргументов для поиска.</param>
		/// <param name="concurrencyKey">Ключ для последовательного вывода конкурируюих задач.</param>
		/// <param name="nextTryTime">Время запуска обработчика задачи.</param>
		/// <param name="priority">Приоритет обработки задачи.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Уникальный идентификатор задания.</returns>
		Task<string> EnsureAsync<TArgs>(string queueName, TArgs args, string concurrencyKey, DateTime nextTryTime,
			BackgroundJobPriority priority = BackgroundJobPriority.Normal, CancellationToken cancellationToken = default);

		/// <summary>
		///     Удаляет из указанной очереди задачу отложенной обработки с указанными аргументами.
		/// </summary>
		/// <param name="jobId">Идентификатор удаляемой задачи.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns><c>True</c> on a successfull state transition, <c>false</c> otherwise.</returns>
		Task<bool> DeleteAsync(string jobId, CancellationToken cancellationToken = default);

		/// <summary>
		///     Удаляет из указанной посредством атрибута <see cref="BackgroundJobAttribute" /> очереди или очереди по умолчанию
		///     задачу отложенной обработки с указанными аргументами.
		/// </summary>
		/// <typeparam name="TArgs">Тип аргументов для задачи.</typeparam>
		/// <param name="args">Значения аргументов для поиска.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Значение true, если задача была найдена и удалена.</returns>
		Task<bool> DeleteAsync<TArgs>(TArgs args, CancellationToken cancellationToken = default);

		/// <summary>
		///     Удаляет из указанной очереди задачу отложенной обработки с указанными аргументами.
		/// </summary>
		/// <typeparam name="TArgs">Тип аргументов для задачи.</typeparam>
		/// <param name="queueName">Имя очереди, в которой осуществляется поиск.</param>
		/// <param name="args">Значения аргументов для поиска.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Значение true, если задача была найдена и удалена.</returns>
		Task<bool> DeleteAsync<TArgs>(string queueName, TArgs args, CancellationToken cancellationToken = default);

		/// <summary>
		///     Отменяет ранее запущенную задачу отложенной обработки.
		/// </summary>
		/// <param name="jobId">Идентификатор отменяемой задачи.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns></returns>
		Task<bool> CancelAsync(string jobId, CancellationToken cancellationToken = default);

		/// <summary>
		///     Возвращает True если задача еще не выполнена.
		/// </summary>
		/// <param name="jobId">Идентификатор задачи.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns></returns>
		Task<bool> IsNewOrPendingAsync(string jobId, CancellationToken cancellationToken = default);
	}
}
