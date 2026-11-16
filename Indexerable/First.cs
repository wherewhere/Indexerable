using System;
using System.Collections.Generic;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        /// <summary>
        /// Returns the first element of a sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The <see cref="IReadOnlyList{T}"/> to return the first element of.</param>
        /// <returns>The first element in <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> is empty.</exception>
        public static TSource First<TSource>(this IReadOnlyList<TSource> source)
        {
            TSource? first = source.TryGetFirst(out bool found);
            if (!found)
            {
                ThrowHelper.ThrowNoElementsException();
            }
            return first!;
        }

        /// <summary>
        /// Returns the first element of a sequence, or a default value if the sequence contains no elements.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The <see cref="IReadOnlyList{T}"/> to return the first element of.</param>
        /// <returns><see langword="default"/> if <paramref name="source"/> is empty; otherwise, the first element in <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        public static TSource? FirstOrDefault<TSource>(this IReadOnlyList<TSource> source) => source.TryGetFirst(out _);

        /// <summary>
        /// Returns the first element of a sequence, or a default value if the sequence contains no elements.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source">The <see cref="IReadOnlyList{T}" /> to return the first element of.</param>
        /// <param name="defaultValue">The default value to return if the sequence is empty.</param>
        /// <returns><paramref name="defaultValue" /> if <paramref name="source" /> is empty; otherwise, the first element in <paramref name="source" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source" /> is <see langword="null" />.</exception>
        public static TSource FirstOrDefault<TSource>(this IReadOnlyList<TSource> source, TSource defaultValue)
        {
            TSource? first = source.TryGetFirst(out bool found);
            return found ? first! : defaultValue;
        }

        private static TSource? TryGetFirst<TSource>(this IReadOnlyList<TSource> source, out bool found)
        {
            if (source is null) { ThrowHelper.ThrowArgumentNullException(nameof(source)); }
            if (source.Count > 0)
            {
                found = true;
                return source[0];
            }
            found = false;
            return default;
        }
    }
}
