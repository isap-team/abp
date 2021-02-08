using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Isap.CommonCore.Extensions;

// ReSharper disable VirtualMemberCallInConstructor

namespace Isap.CommonCore
{
	[DataContract]
	public class ResultSet
	{
		public ResultSet(IEnumerable data, int pageNumber, int pageSize, int? totalCount = null)
		{
			List<IList> chunks = data.ChunkBy(pageSize).Take(2).ToList();
			switch (chunks.Count)
			{
				case 0:
					SetData(new ArrayList());
					IsLastPage = true;
					break;
				case 1:
					SetData(chunks[0]);
					IsLastPage = true;
					break;
				case 2:
					SetData(chunks[0]);
					IsLastPage = false;
					break;
			}

			PageNumber = pageNumber;
			PageSize = pageSize;
			TotalCount = totalCount;
		}

		public ResultSet()
		{
		}

		[DataMember]
		public IList Data { get; set; }

		[DataMember]
		public int PageNumber { get; set; }

		[DataMember]
		public int PageSize { get; set; }

		[DataMember]
		public int? TotalCount { get; set; }

		public bool IsFirstPage => PageNumber == 1;
		public bool HasPrevPage => PageNumber > 1;
		public bool HasNextPage => !IsLastPage;

		[DataMember]
		public bool IsLastPage { get; set; }

		public int? PageCount => TotalCount.HasValue && PageSize > 0 ? (TotalCount.Value + PageSize - 1) / PageSize : (int?) null;
		public bool IsEmpty => Data == null || Data.Count == 0;

		protected virtual void SetData(IList data)
		{
			Data = data;
		}

		public ResultSet<TResult> Convert<TResult>(Func<object, TResult> convert)
		{
			return new ResultSet<TResult>(Data.Cast<object>().Select(convert).ToList(), PageNumber, PageSize, TotalCount)
				{
					IsLastPage = IsLastPage,
				};
		}
	}

	[DataContract]
	public class ResultSet<T>: ResultSet
	{
		public ResultSet(IEnumerable<T> data, int pageNumber, int pageSize, int? totalCount = null)
			: base(data, pageNumber, pageSize, totalCount)
		{
		}

		public ResultSet()
		{
		}

		[DataMember]
		[XmlIgnore]
		public new List<T> Data
		{
			get => (List<T>) base.Data;
			set => SetData(value);
		}

		protected override void SetData(IList data)
		{
			base.SetData(data.Cast<T>().ToList());
		}

		public ResultSet<TResult> Convert<TResult>(Func<T, TResult> convert)
		{
			List<TResult> result = Data.Select(convert).ToList();
			return new ResultSet<TResult>(result, PageNumber, PageSize, TotalCount)
				{
					IsLastPage = IsLastPage,
				};
		}

		public async Task<ResultSet<TResult>> ConvertAsync<TResult>(Func<T, Task<TResult>> convert)
		{
			List<TResult> result = new List<TResult>();
			foreach (T entry in Data)
			{
				TResult convertedEntry = await convert(entry);
				result.Add(convertedEntry);
			}

			return new ResultSet<TResult>(result, PageNumber, PageSize, TotalCount)
				{
					IsLastPage = IsLastPage,
				};
		}
	}
}
