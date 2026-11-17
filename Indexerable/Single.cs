using System;
using System.Collections.Generic;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        /// <summary>
        /// Returns the only element of a sequence, and throws an exception if there is not exactly one element in the sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence to return the single element of.</param>
        /// <returns>The single element of the input sequence.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> is empty or contains more than one element.</exception>
        public static TSource Single<TSource>(this IReadOnlyList<TSource> source)
        {
            TSource? single = source.TryGetSingle(out bool found);
            if (!found)
            {
                ThrowHelper.ThrowNoElementsException();
            }
            return single!;
        }

        /// <summary>
        /// Returns the only element of a sequence, or a default value if the sequence is empty;
        /// this method throws an exception if there is more than one element in the sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence to return the single element of.</param>
        /// <returns>The single element of the input sequence, or <see langword="default"/> if the sequence contains no elements.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains more than one element.</exception>
        public static TSource? SingleOrDefault<TSource>(this IReadOnlyList<TSource> source) => source.TryGetSingle(out _);

        /// <summary>
        /// Returns the only element of a sequence, or a default value if the sequence is empty;
        /// this method throws an exception if there is more than one element in the sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence to return the single element of.</param>
        /// <param name="defaultValue">The default value to return if the sequence is empty.</param>
        /// <returns>The single element of the input sequence, or <paramref name="defaultValue"/> if the sequence contains no elements.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">The input sequence contains more than one element.</exception>
        public static TSource SingleOrDefault<TSource>(this IReadOnlyList<TSource> source, TSource defaultValue)
        {
            TSource? single = source.TryGetSingle(out bool found);
            return found ? single! : defaultValue;
        }

        private static TSource? TryGetSingle<TSource>(this IReadOnlyList<TSource> source, out bool found)
        {
            if (source is null) { ThrowHelper.ThrowArgumentNullException(nameof(source)); }
            switch (source.Count)
            {
                case 0:
                    found = false;
                    return default;
                case 1:
                    found = true;
                    return source[0];
            }
            found = false;
            ThrowHelper.ThrowMoreThanOneElementException();
            return default;
        }
    }
}
