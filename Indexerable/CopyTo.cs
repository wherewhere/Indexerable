using System;
using System.Collections.Generic;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        /// <summary>
        /// Copies the elements of the <paramref name="source"/> to <paramref name="array"/>,
        /// starting at the specified destination index.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in <paramref name="source"/>.</typeparam>
        /// <param name="source">The read-only list whose elements are copied.</param>
        /// <param name="array">The one-dimensional, zero-based destination array.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than zero.</exception>
        /// <exception cref="ArgumentException">The number of elements in <paramref name="source"/> is greater than the available space from
        /// <paramref name="arrayIndex"/> to the end of <paramref name="array"/>.
        /// </exception>
        public static void CopyTo<TSource>(this IReadOnlyList<TSource> source, TSource[] array, int arrayIndex)
        {
            if (source is null) { ThrowHelper.ThrowArgumentNullException(nameof(source)); }
            if (array is null) { ThrowHelper.ThrowArgumentNullException(nameof(array)); }
            if (source is ICollection<TSource> collection)
            {
                collection.CopyTo(array, arrayIndex);
            }
            else
            {
                for (int i = 0; i < source.Count; i++)
                {
                    array[arrayIndex + i] = source[i];
                }
            }
        }
    }
}
