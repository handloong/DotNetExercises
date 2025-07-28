using System;
using System.Collections.Generic;
using System.Linq;

namespace PostProcessing.DefectsClustering
{
	public static class DefectsClusteringExtensions
	{
		public static IOrderedEnumerable<TSource> OrderByWithDirection<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer, bool descending)
		{
			return descending ? source.OrderByDescending(keySelector, comparer) : source.OrderBy(keySelector, comparer);
		}

		public static IOrderedEnumerable<TSource> ThenByWithDirection<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, bool descending)
		{
			return descending ? source.ThenByDescending(keySelector) : source.ThenBy(keySelector);
		}
	}
}
