using System;
using Newtonsoft.Json.Linq;

namespace Isap.CommonCore.Services
{
	/// <summary>
	///     Значения для фильтра.
	/// </summary>
	public class DataFilterValueDto
	{
		public DataFilterValueDto(Guid dataFilterId, JToken values)
		{
			DataFilterId = dataFilterId;
			Values = values;
		}

		public DataFilterValueDto()
		{
		}

		/// <summary>
		///     Идентификатор записи о фильтре.
		/// </summary>
		public Guid DataFilterId { get; set; }

		/// <summary>
		///     Набор значений (<see cref="System.Collections.Generic.Dictionary{TKey,TValue}" />).
		/// </summary>
		public JToken Values { get; set; }
	}
}
