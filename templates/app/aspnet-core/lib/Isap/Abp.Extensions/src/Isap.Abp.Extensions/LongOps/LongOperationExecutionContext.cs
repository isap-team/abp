using System.Threading.Tasks;

namespace Isap.Abp.Extensions.LongOps
{
	public interface ILongOperationExecutionContext
	{
		/// <summary>
		///     Общее количество единиц, требующих обработки.
		/// </summary>
		int TotalCount { get; }

		/// <summary>
		///     Количество обработанных единиц.
		/// </summary>
		int ProcessedCount { get; }

		/// <summary>
		///     В сотых долях процента. Например значение 1000 соответствует 10%, а 10000 соответствует 100%.
		/// </summary>
		int CurrentPercent { get; }

		void UpdateProgress(int totalCount, int processedCount);
		Task UpdateProgressAsync(int totalCount, int processedCount);
	}

	public class LongOperationExecutionContext: ILongOperationExecutionContext
	{
		public int TotalCount { get; private set; }
		public int ProcessedCount { get; private set; }
		public int CurrentPercent => TotalCount == 0 ? 0 : ProcessedCount * 10000 / TotalCount;

		public virtual void UpdateProgress(int totalCount, int processedCount)
		{
			TotalCount = totalCount;
			ProcessedCount = processedCount;
		}

		public virtual async Task UpdateProgressAsync(int totalCount, int processedCount)
		{
			await Task.Yield();
			UpdateProgress(totalCount, processedCount);
		}
	}
}
