using System;
using System.Collections.Generic;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        /// <summary>
        /// Returns the last element of a sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The <see cref="IReadOnlyList{T}"/> to return the last element of.</param>
        /// <returns>The last element in <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> is empty.</exception>
        public static TSource Last<TSource>(this IReadOnlyList<TSource> source)
        {
            TSource? last = source.TryGetLast(out bool found);
            if (!found) { ThrowHelper.ThrowNoElementsException(); }
            return last!;
        }

        /// <summary>
        /// Returns the last element of a sequence that satisfies a condition.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The <see cref="IReadOnlyList{T}"/> to return an element from.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The last element in <paramref name="source"/> that passes the test in <paramref name="predicate"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="predicate"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">No element satisfies the condition in <paramref name="predicate"/>, or <paramref name="source"/> is empty.</exception>
        public static TSource Last<TSource>(this IReadOnlyList<TSource> source, Func<TSource, bool> predicate)
        {
            TSource? last = source.TryGetLast(predicate, out bool found);
            if (!found) { ThrowHelper.ThrowNoMatchException(); }
            return last!;
        }

        /// <summary>
        /// Returns the last element of a sequence, or a default value if the sequence contains no elements.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The <see cref="IReadOnlyList{T}"/> to return the last element of.</param>
        /// <returns><see langword="default"/> if <paramref name="source"/> is empty; otherwise, the last element in <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        public static TSource? LastOrDefault<TSource>(this IReadOnlyList<TSource> source) => source.TryGetLast(out _);

        /// <summary>
        /// Returns the last element of a sequence, or a default value if the sequence contains no elements.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source">An <see cref="IReadOnlyList{T}" /> to return the last element of.</param>
        /// <param name="defaultValue">The default value to return if the sequence is empty.</param>
        /// <returns><paramref name="defaultValue" /> if the source sequence is empty; otherwise, the last element in the <see cref="IReadOnlyList{T}" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source" /> is <see langword="null" />.</exception>
        public static TSource LastOrDefault<TSource>(this IReadOnlyList<TSource> source, TSource defaultValue)
        {
            TSource? last = source.TryGetLast(out bool found);
            return found ? last! : defaultValue;
        }

        /// <summary>
        /// Returns the last element of a sequence that satisfies a condition, or a default value if no such element is found.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The <see cref="IReadOnlyList{T}"/> to return an element from.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns><see langword="default"/> if <paramref name="source"/> is empty or no elements pass the test in <paramref name="predicate"/>; otherwise, the last element that passes the test.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="predicate"/> is <see langword="null"/>.</exception>
        public static TSource? LastOrDefault<TSource>(this IReadOnlyList<TSource> source, Func<TSource, bool> predicate) => source.TryGetLast(predicate, out _);

        /// <summary>
        /// Returns the last element of a sequence that satisfies a condition or a default value if no such element is found.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source">An <see cref="IReadOnlyList{T}" /> to return an element from.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="defaultValue">The default value to return if the sequence is empty.</param>
        /// <returns><paramref name="defaultValue" /> if the sequence is empty or if no elements pass the test in the predicate function; otherwise, the last element that passes the test in the predicate function.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source" /> or <paramref name="predicate" /> is <see langword="null" />.</exception>
        public static TSource LastOrDefault<TSource>(this IReadOnlyList<TSource> source, Func<TSource, bool> predicate, TSource defaultValue)
        {
            TSource? last = source.TryGetLast(predicate, out bool found);
            return found ? last! : defaultValue;
        }

        private static TSource? TryGetLast<TSource>(this IReadOnlyList<TSource> source, out bool found)
        {
            if (source is null) { ThrowHelper.ThrowArgumentNullException(nameof(source)); }
            int count = source.Count;
            if (count > 0)
            {
                found = true;
                return source[count - 1];
            }
            found = false;
            return default;
        }

        private static TSource? TryGetLast<TSource>(this IReadOnlyList<TSource> source, Func<TSource, bool> predicate, out bool found)
        {
            if (source is null) { ThrowHelper.ThrowArgumentNullException(nameof(source)); }
            if (predicate is null) { ThrowHelper.ThrowArgumentNullException(nameof(predicate)); }
            for (int i = source.Count - 1; i >= 0; --i)
            {
                TSource result = source[i];
                if (predicate(result))
                {
                    found = true;
                    return result;
                }
            }
            found = false;
            return default;
        }
    }
}
