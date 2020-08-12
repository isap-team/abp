namespace Isap.CommonCore.Services
{
	/// <summary>
	///     Настройка сортировки результатов выборки по полю.
	/// </summary>
	public class SortOptionDto
	{
		/// <summary>
		///     Имя поля для сортировки.
		/// </summary>
		public string FieldName { get; set; }

		/// <summary>
		///     True, если нужна сортировка по убыванию.
		/// </summary>
		public bool IsDescending { get; set; }
	}
}
