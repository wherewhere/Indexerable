using System;
using System.Collections.Generic;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        public static int IndexOf<TSource>(this IReadOnlyList<TSource> source, TSource value)
        {
            if (source is null) { ThrowHelper.ThrowArgumentNullException(nameof(source)); }
            else if (source is List<TSource> list)
            {
                return list.IndexOf(value);
            }
            else
            {
                for (int i = 0; i < source.Count; i++)
                {
                    if (EqualityComparer<TSource>.Default.Equals(source[i], value))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public static int IndexOf<TSource>(this IReadOnlyList<TSource> source, Func<TSource, bool> predicate)
        {
            if (source is null) { ThrowHelper.ThrowArgumentNullException(nameof(source)); }
            if (predicate is null) { ThrowHelper.ThrowArgumentNullException(nameof(predicate)); }
            switch (source)
            {
                case List<TSource> list:
                    return list.FindIndex(predicate.Invoke);
                case TSource[] array:
                    return Array.FindIndex(array, predicate.Invoke);
#if NETCOREAPP || NETCORE5_0
                case System.Collections.Immutable.ImmutableList<TSource> immutableList:
                    return immutableList.FindIndex(predicate.Invoke);
#endif
                default:
                    for (int i = 0; i < source.Count; i++)
                    {
                        if (predicate(source[i]))
                        {
                            return i;
                        }
                    }
                    return -1;
            }
        }
    }
}
