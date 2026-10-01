using System;
using System.Collections.Generic;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        /// <summary>
        /// Searches for the specified value and returns its zero-based index in a read-only list.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The list to search.</param>
        /// <param name="value">The value to locate in <paramref name="source"/>.</param>
        /// <returns>The zero-based index of the first occurrence of <paramref name="value"/>, if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
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

        /// <summary>
        /// Searches for an element that matches a predicate and returns its zero-based index.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The list to search.</param>
        /// <param name="predicate">A function that determines whether an element matches the search criteria.</param>
        /// <returns>The zero-based index of the first element that matches <paramref name="predicate"/>, if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="predicate"/> is <see langword="null"/>.</exception>
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
