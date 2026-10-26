using System;
using System.Collections.Generic;

#if NETCOREAPP || NETCORE5_0
using System.Collections.Immutable;
#endif

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
            switch (source)
            {
                case null:
                    ThrowHelper.ThrowArgumentNullException(nameof(source));
                    break;
                case IList<TSource> list:
                    return list.IndexOf(value);
#if NETCOREAPP || NETCORE5_0
                case IImmutableList<TSource> immutableList:
                    return immutableList.IndexOf(item: value);
#endif
                default:
                    EqualityComparer<TSource> equalityComparer = EqualityComparer<TSource>.Default;
                    for (int i = 0; i < source.Count; i++)
                    {
                        if (equalityComparer.Equals(source[i], value))
                        {
                            return i;
                        }
                    }
                    break;
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
        public static int IndexOf<TSource>(this IReadOnlyList<TSource> source, Predicate<TSource> predicate)
        {
            if (source is null) { ThrowHelper.ThrowArgumentNullException(nameof(source)); }
            if (predicate is null) { ThrowHelper.ThrowArgumentNullException(nameof(predicate)); }
            switch (source)
            {
                case List<TSource> list:
                    return list.FindIndex(predicate);
                case TSource[] array:
                    return Array.FindIndex(array, predicate);
#if NETCOREAPP || NETCORE5_0
                case ImmutableList<TSource> immutableList:
                    return immutableList.FindIndex(predicate);
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
