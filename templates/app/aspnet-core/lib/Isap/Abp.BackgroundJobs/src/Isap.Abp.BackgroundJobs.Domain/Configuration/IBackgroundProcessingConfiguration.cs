using System;
using System.Collections.Generic;

namespace Isap.Abp.BackgroundJobs.Configuration
{
	public interface IBackgroundProcessingConfiguration
	{
		/// <summary>
		///     Включает/выключает планировщик задач отложенной обработки.
		/// </summary>
		bool IsEnabled { get; }

		/// <summary>
		///     Имя очереди "по-умолчанию". Для задач, которые ставятся в очередь посредством метода EnqueueAsync, в сигнатуре
		///     которого отсутствует параметр принимающий имя очереди.
		/// </summary>
		string DefaultQueueName { get; }

		/// <summary>
		///     Интервал ожидания активности от процессора очереди, после которого задача может быть передана на обработку другим
		///     процессором.
		/// </summary>
		TimeSpan ProcessorInactiveTimeout { get; }

		/// <summary>
		///     Интервал по истечении которого информация о ранее завершенной задаче будет навсегда удалена из базы.
		/// </summary>
		TimeSpan ObsoleteJobRemovingTimeout { get; }

		/// <summary>
		///     Конфигурация очередей зада отложенной обработки.
		/// </summary>
		ICollection<IJobQueueConfiguration> Queues { get; }
	}
}
