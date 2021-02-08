namespace Isap.Abp.Extensions.DataFilters
{
	public enum DataFilterType
	{
		SelectOne = 0,
		SelectMany = 1,
		SelectRange = 2,
		SearchLike = 3,
		IsAnyOrOne = 4,
		IsAnyOrRange = 5,
		SearchLikeWithExact = 6,

		/// <summary>
		///     Поиск с учетом положения пробела.
		/// </summary>
		SearchLikeSpace = 7,
	}
}
