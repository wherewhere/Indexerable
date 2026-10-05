using System;
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

        private interface ISkip<out TSource> : IReadOnlyList<TSource>
        {
            /// <inheritdoc cref="Skip{TSource}(IReadOnlyList{TSource}, int)"/>
            IReadOnlyList<TSource> Skip(int count);
        }

        private sealed partial class SkipIndexer<TSource>(IReadOnlyList<TSource> source, int count) : SkipIndexerBase<TSource>(source), ISkipBoth<TSource>
        {
            public override int Count => Math.Max(0, source.Count - count);
            public IReadOnlyList<TSource> Skip(int _count) => _count <= 0 ? this : new SkipIndexer<TSource>(source, count + _count);
            public IReadOnlyList<TSource> SkipLast(int _count) => _count <= 0 ? this : new SkipBothIndexer<TSource>(source, count, _count);
            protected override int GetCount(out int offset)
            {
                offset = count;
                return Count;
            }
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

        private interface ISkipLast<out TSource> : IReadOnlyList<TSource>
        {
            /// <inheritdoc cref="SkipLast{TSource}(IReadOnlyList{TSource}, int)"/>
            IReadOnlyList<TSource> SkipLast(int count);
        }

        private sealed partial class SkipLastIndexer<TSource>(IReadOnlyList<TSource> source, int count) : TakeIndexerBase<TSource>(source), ISkipBoth<TSource>
        {
            public override int Count => Math.Max(0, source.Count - count);

            public IReadOnlyList<TSource> Skip(int _count) => _count <= 0 ? this : new SkipBothIndexer<TSource>(source, _count, count);

            public IReadOnlyList<TSource> SkipLast(int _count) => _count <= 0 ? this : new SkipLastIndexer<TSource>(source, count + _count);
        }

        private interface ISkipBoth<out TSource> : ISkip<TSource>, ISkipLast<TSource>;

        private sealed partial class SkipBothIndexer<TSource>(IReadOnlyList<TSource> source, int skip, int skipLast) : IndexerBase<TSource>, ISkipBoth<TSource>
        {
            public override TSource this[int index] =>
                index >= 0 && (uint)index < (uint)Count
                    ? source[index + skip]
                    : throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

            public override int Count => Math.Max(0, source.Count - skip - skipLast);

            public IReadOnlyList<TSource> Skip(int _count) => _count <= 0 ? this : new SkipBothIndexer<TSource>(source, skip + _count, skipLast);
            public IReadOnlyList<TSource> SkipLast(int _count) => _count <= 0 ? this : new SkipBothIndexer<TSource>(source, skip, skipLast + _count);

            public override bool Contains(TSource item)
            {
                int count = Count;
                for (int i = 0; i < count; i++)
                {
                    TSource j = source[i + skip];
                    if (EqualityComparer<TSource>.Default.Equals(j, item))
                    {
                        return true;
                    }
                }
                return false;
            }

            public override int IndexOf(TSource item)
            {
                int count = Count;
                for (int i = 0; i < count; i++)
                {
                    TSource j = source[i + skip];
                    if (EqualityComparer<TSource>.Default.Equals(j, item))
                    {
                        return i;
                    }
                }
                return -1;
            }

            public override void CopyTo(TSource[] array, int arrayIndex)
            {
                int count = Count;
                for (int i = 0; i < count; i++)
                {
                    array[arrayIndex + i] = source[i + skip];
                }
            }

            public override IEnumerator<TSource> GetEnumerator() => new IndexerEnumerator(source, Count, skip);

            private sealed partial class IndexerEnumerator(IReadOnlyList<TSource> source, int count, int offset) : IndexerEnumeratorBase
            {
                public override TSource Current => source[offset + _index];

                public override bool MoveNext()
                {
                    int index = _index + 1;
                    if ((uint)index < (uint)count)
                    {
                        _index = index;
                        return true;
                    }
                    _index = count;
                    return false;
                }
            }
        }
    }
}
