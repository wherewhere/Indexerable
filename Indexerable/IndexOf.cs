using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        public static int IndexOf<TSource>(this IReadOnlyList<TSource> source, TSource value)
        {
            ArgumentNullException.ThrowIfNull(source);
            if (source is List<TSource> list)
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
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(predicate);
            switch (source)
            {
                case List<TSource> list:
                    return list.FindIndex(predicate.Invoke);
                case TSource[] array:
                    return Array.FindIndex(array, predicate.Invoke);
                case ImmutableList<TSource> immutableList:
                    return immutableList.FindIndex(predicate.Invoke);
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
