using System;
using System.Collections;
using System.Collections.Generic;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        /// <summary>
        /// Bypasses a specified number of elements in a read-only list and returns the remaining elements.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The list to return elements from.</param>
        /// <param name="count">The number of elements to skip before returning the remaining elements.</param>
        /// <returns>A read-only list that contains the elements that occur after the specified number of elements in <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <remarks>If <paramref name="count"/> is less than or equal to zero, all elements of <paramref name="source"/> are returned. If <paramref name="source"/> contains fewer than <paramref name="count"/> elements, the result is empty.</remarks>
        public static IReadOnlyList<TSource> Skip<TSource>(this IReadOnlyList<TSource> source, int count)
        {
            if (source is null) { ThrowHelper.ThrowArgumentNullException(nameof(source)); }
            return count <= 0 ? source
                : source is ISkip<TSource> skip ? skip.Skip(count)
                : new SkipIndexer<TSource>(source, count);
        }

        private interface ISkip<TSource> : IReadOnlyList<TSource>
        {
            /// <inheritdoc cref="Skip{TSource}(IReadOnlyList{TSource}, int)"/>
            IReadOnlyList<TSource> Skip(int count);
        }

        private sealed partial class SkipIndexer<TSource>(IReadOnlyList<TSource> source, int count) : ISkip<TSource>
        {
            public TSource this[int index] =>
                index >= 0 && index < Count
                    ? source[index + count]
                    : throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

            public int Count => Math.Max(0, source.Count - count);

            public IReadOnlyList<TSource> Skip(int _count) => _count <= 0 ? this : new SkipIndexer<TSource>(source, count + _count);

            public IEnumerator<TSource> GetEnumerator()
            {
                for (int i = 0; i < Count; i++)
                {
                    yield return source[i + count];
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        /// <summary>
        /// Bypasses a specified number of elements at the end of a read-only list and returns the remaining elements.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The list to return elements from.</param>
        /// <param name="count">The number of elements to skip from the end of the list.</param>
        /// <returns>A read-only list that contains the elements of <paramref name="source"/> except for the specified number of elements at the end.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <remarks>If <paramref name="count"/> is less than or equal to zero, all elements of <paramref name="source"/> are returned. If <paramref name="count"/> is greater than or equal to the number of elements in <paramref name="source"/>, the result is empty.</remarks>
        public static IReadOnlyList<TSource> SkipLast<TSource>(this IReadOnlyList<TSource> source, int count)
        {
            if (source is null) { ThrowHelper.ThrowArgumentNullException(nameof(source)); }
            return count <= 0 ? source
                : source is ISkipLast<TSource> skip ? skip.SkipLast(count)
                : new SkipLastIndexer<TSource>(source, count);
        }

        private interface ISkipLast<TSource> : IReadOnlyList<TSource>
        {
            /// <inheritdoc cref="SkipLast{TSource}(IReadOnlyList{TSource}, int)"/>
            IReadOnlyList<TSource> SkipLast(int count);
        }

        private sealed partial class SkipLastIndexer<TSource>(IReadOnlyList<TSource> source, int count) : ISkipLast<TSource>
        {
            public TSource this[int index] =>
                index >= 0 && index < Count
                        ? source[index]
                        : throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

            public int Count => Math.Max(0, source.Count - count);

            public IReadOnlyList<TSource> SkipLast(int _count) => _count <= 0 ? this : new SkipLastIndexer<TSource>(source, count + _count);

            public IEnumerator<TSource> GetEnumerator()
            {
                for (int i = 0; i < Count; i++)
                {
                    yield return source[i];
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
