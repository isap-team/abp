using System;
using System.Collections.Generic;
using System.Linq;
using Isap.CommonCore.Extensions;
using JetBrains.Annotations;

namespace Isap.Abp.Extensions.Domain
{
	public static class EntityExtensions
	{
		public static void Merge<TSourceItem, TDestinationItem, TMergeKey>([NotNull] this ICollection<TDestinationItem> destination,
			ICollection<TSourceItem> source, Func<TDestinationItem, TMergeKey> getDestinationKey, Func<TSourceItem, TMergeKey> getSourceKey,
			Action<TDestinationItem, TSourceItem> assign,
			Func<TSourceItem, TDestinationItem> create)
		{
			if (destination == null) throw new ArgumentNullException(nameof(destination));

			if (source == null) return;

			List<Tuple<TMergeKey, TDestinationItem, TSourceItem>> joinedItems = destination
				.FullOuterJoin(source, getDestinationKey, getSourceKey, Tuple.Create)
				.ToList();

			foreach (Tuple<TMergeKey, TDestinationItem, TSourceItem> joinedItem in joinedItems)
			{
				//TMergeKey mergeKey = joinedItem.Item1;
				TDestinationItem destinationItem = joinedItem.Item2;
				TSourceItem sourceItem = joinedItem.Item3;

				if (Equals(destinationItem, default))
				{
					destinationItem = create(sourceItem);
					assign(destinationItem, sourceItem);
					destination.Add(destinationItem);
				}
				else if (Equals(sourceItem, default))
				{
					destination.Remove(destinationItem);
				}
				else
				{
					assign(destinationItem, sourceItem);
				}
			}
		}

		public static void Merge<TSourceItem, TDestinationItem, TMergeKey>([NotNull] this ICollection<TDestinationItem> destination,
			ICollection<TSourceItem> source, Func<TDestinationItem, TMergeKey> getDestinationKey, Func<TSourceItem, TMergeKey> getSourceKey,
			Action<TDestinationItem, TSourceItem> assign)
			where TDestinationItem: new()
		{
			Merge(destination, source, getDestinationKey, getSourceKey, assign, src => new TDestinationItem());
		}
	}
}
