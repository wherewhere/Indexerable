using System;
using System.Collections.Generic;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        /// <summary>
        /// Generates a read-only list of sequential integers within a specified range.
        /// </summary>
        /// <param name="start">The value of the first integer in the list.</param>
        /// <param name="count">The number of sequential integers to generate.</param>
        /// <returns>A read-only list containing <paramref name="count"/> sequential integers starting at <paramref name="start"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is less than zero, or the last value in the range is greater than <see cref="int.MaxValue"/>.</exception>
        public static IReadOnlyList<int> Range(int start, int count)
        {
            long max = ((long)start) + count - 1;
            if (count < 0 || max > int.MaxValue)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(count));
            }
            return count switch
            {
                0 => new EmptyIndexer<int>(),
                1 => new SingleElementIndexer<int>(start),
                _ => start switch
                {
                    0 => new RangeIndexer(count),
                    _ => new RangeOffsetIndexer(start, count)
                }
            };
        }

        private abstract partial class RangeIndexerBase<TSource>(int count) : IndexerBase<TSource>
        {
            protected readonly int count = count;
            public sealed override int Count => count;
            protected abstract partial class RangeIndexerEnumerator(int count) : IndexerEnumeratorBase
            {
                public sealed override bool MoveNext()
                {
                    int index = _index + 1, length = count;
                    if ((uint)index < (uint)length)
                    {
                        _index = index;
                        return true;
                    }
                    _index = length;
                    return false;
                }
            }
        }

        /// <summary>
        /// An iterator that yields a range of consecutive integers.
        /// </summary>
        private sealed partial class RangeIndexer(int count) : RangeIndexerBase<int>(count), ISelectWithIndex<int>, ISkipBoth<int>, ITake<int>, ITakeLast<int>
        {
            public override int this[int index] =>
                index >= 0 && index < count
                    ? index
                    : throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

            public IReadOnlyList<TResult> Select<TResult>(Func<int, TResult> selector) =>
                new CreateIndexer<TResult>(count, selector);

            public IReadOnlyList<TResult> Select<TResult>(Func<int, int, TResult> selector) =>
                new CreateIndexer<TResult>(count, i => selector(i, i));

            public IReadOnlyList<int> Skip(int _count) =>
                _count <= 0 ? this
                    : _count >= count ? new EmptyIndexer<int>()
                    : new RangeOffsetIndexer(_count, count - _count);

            public IReadOnlyList<int> SkipLast(int _count) =>
                _count <= 0 ? this
                    : _count >= count ? new EmptyIndexer<int>()
                    : new RangeIndexer(count - _count);

            public IReadOnlyList<int> Take(int _count) =>
                _count <= 0 ? new EmptyIndexer<int>()
                    : _count >= count ? this
                    : new RangeIndexer(_count);

            public IReadOnlyList<int> TakeLast(int _count) =>
                _count <= 0 ? new EmptyIndexer<int>()
                    : _count >= count ? this
                    : new RangeOffsetIndexer(count - _count, _count);

            public override bool Contains(int item) => item >= 0 && item < count;

            public override int IndexOf(int item) => item >= 0 && item < count ? item : -1;

            public override void CopyTo(int[] array, int arrayIndex)
            {
                for (int i = 0; i < count; i++)
                {
                    array[arrayIndex++] = i;
                }
            }

            public override IEnumerator<int> GetEnumerator() => new IndexerEnumerator(count);

            private sealed partial class IndexerEnumerator(int count) : RangeIndexerEnumerator(count)
            {
                public override int Current => _index;
            }
        }

        private abstract partial class RangeOffsetIndexerBase<TSource>(int offset, int count) : RangeIndexerBase<TSource>(count)
        {
            protected readonly int offset = offset;

            protected abstract partial class RangeOffsetIndexerEnumerator(int offset, int count) : IndexerEnumeratorBase
            {
                private readonly int offset = offset;
                protected int current = offset;

                public sealed override bool MoveNext()
                {
                    if (++_index == 0)
                    {
                        return true;
                    }
                    else if (_index < count)
                    {
                        current++;
                        return true;
                    }
                    return false;
                }

                public sealed override void Reset()
                {
                    base.Reset();
                    current = offset;
                }
            }
        }

        /// <summary>
        /// An iterator that yields a range of consecutive integers.
        /// </summary>
        private sealed partial class RangeOffsetIndexer(int offset, int count) : RangeOffsetIndexerBase<int>(offset, count), ISelectWithIndex<int>, ISkipBoth<int>, ITake<int>, ITakeLast<int>
        {
            public override int this[int index] =>
                index >= 0 && index < count
                    ? offset + index
                    : throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

            public IReadOnlyList<TResult> Select<TResult>(Func<int, TResult> selector) =>
                new CreateOffsetIndexer<TResult>(offset, count, selector);

            public IReadOnlyList<TResult> Select<TResult>(Func<int, int, TResult> selector) =>
                new CreateOffsetIndexer<TResult>(offset, count, i => selector(i, i - offset));

            public IReadOnlyList<int> Skip(int _count) =>
                _count <= 0 ? this
                    : _count >= count ? new EmptyIndexer<int>()
                    : new RangeOffsetIndexer(offset + _count, count - _count);

            public IReadOnlyList<int> SkipLast(int _count) =>
                _count <= 0 ? this
                    : _count >= count ? new EmptyIndexer<int>()
                    : new RangeOffsetIndexer(offset, count - _count);

            public IReadOnlyList<int> Take(int _count) =>
                _count <= 0 ? new EmptyIndexer<int>()
                    : _count >= count ? this
                    : new RangeOffsetIndexer(offset, _count);

            public IReadOnlyList<int> TakeLast(int _count) =>
                _count <= 0 ? new EmptyIndexer<int>()
                    : _count >= count ? this
                    : new RangeOffsetIndexer(offset + (count - _count), _count);

            public override bool Contains(int item) => item >= offset && item < offset + count;

            public override int IndexOf(int item)
            {
                int sub = item - offset;
                return sub >= 0 && sub < count ? sub : -1;
            }

            public override void CopyTo(int[] array, int arrayIndex)
            {
                int i = count;
                for (int current = offset; i > 0; i--)
                {
                    array[arrayIndex++] = current++;
                }
            }

            public override IEnumerator<int> GetEnumerator() => new IndexerEnumerator(offset, count);

            private sealed partial class IndexerEnumerator(int start, int count) : RangeOffsetIndexerEnumerator(start, count)
            {
                public override int Current => current;
            }
        }
    }
}
