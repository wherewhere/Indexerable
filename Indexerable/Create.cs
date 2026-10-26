using System;
using System.Collections.Generic;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        /// <summary>
        /// Creates a read-only list whose elements are produced by a <paramref name="selector"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of the elements in the list.</typeparam>
        /// <param name="count">The number of elements in the list.</param>
        /// <param name="selector">A function that produces an element for a zero-based index.</param>
        /// <returns>A read-only list containing the selected elements.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is less than zero.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
        public static IReadOnlyList<TResult> Create<TResult>(int count, Func<int, TResult> selector)
        {
            if (count < 0) { ThrowHelper.ThrowArgumentOutOfRangeException(nameof(count)); }
            if (selector is null) { ThrowHelper.ThrowArgumentNullException(nameof(selector)); }
            return count == 0 ? new EmptyIndexer<TResult>() : new CreateIndexer<TResult>(count, selector);
        }

        private sealed partial class CreateIndexer<TResult>(int count, Func<int, TResult> selector) : RangeIndexerBase<TResult>(count), ISelectWithIndex<TResult>, ISkipBoth<TResult>, ITake<TResult>, ITakeLast<TResult>
        {
            public override TResult this[int index] =>
                index >= 0 && (uint)index < (uint)count
                    ? selector(index)
                    : throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

            public IReadOnlyList<TResult2> Select<TResult2>(Func<TResult, TResult2> _selector) =>
                new CreateIndexer<TResult2>(count, selector.CombineSelectors(_selector));

            public IReadOnlyList<TResult2> Select<TResult2>(Func<TResult, int, TResult2> _selector) =>
                new CreateIndexer<TResult2>(count, selector.CombineSelectors(_selector));

            public IReadOnlyList<TResult> Skip(int _count) =>
                _count <= 0 ? this
                    : (uint)_count >= (uint)count ? new EmptyIndexer<TResult>()
                    : new CreateOffsetIndexer<TResult>(_count, count - _count, selector);

            public IReadOnlyList<TResult> SkipLast(int _count) =>
                _count <= 0 ? this
                    : (uint)_count >= (uint)count ? new EmptyIndexer<TResult>()
                    : new CreateIndexer<TResult>(count - _count, selector);

            public IReadOnlyList<TResult> Take(int _count) =>
                _count <= 0 ? new EmptyIndexer<TResult>()
                    : (uint)_count >= (uint)count ? this
                    : new CreateIndexer<TResult>(_count, selector);

            public IReadOnlyList<TResult> TakeLast(int _count) =>
                _count <= 0 ? new EmptyIndexer<TResult>()
                    : (uint)_count >= (uint)count ? this
                    : new CreateOffsetIndexer<TResult>(count - _count, _count, selector);

            public override bool Contains(TResult item)
            {
                EqualityComparer<TResult> equalityComparer = EqualityComparer<TResult>.Default;
                for (int i = 0; i < count; i++)
                {
                    if (equalityComparer.Equals(selector(i), item))
                    {
                        return true;
                    }
                }
                return false;
            }

            public override int IndexOf(TResult item)
            {
                EqualityComparer<TResult> equalityComparer = EqualityComparer<TResult>.Default;
                for (int i = 0; i < count; i++)
                {
                    if (equalityComparer.Equals(selector(i), item))
                    {
                        return i;
                    }
                }
                return -1;
            }

            public override void CopyTo(TResult[] array, int arrayIndex)
            {
                for (int i = 0; i < count; i++)
                {
                    array[arrayIndex++] = selector(i);
                }
            }

            public sealed override IEnumerator<TResult> GetEnumerator() => new IndexerEnumerator(count, selector);

            private sealed partial class IndexerEnumerator(int count, Func<int, TResult> selector) : RangeIndexerEnumerator(count)
            {
                public override TResult Current => selector(_index);
            }
        }

#if COMP_NETSTANDARD2_1
        private sealed partial class CreateIndexer<TResult> : ITakeRange<TResult>
        {
            public IReadOnlyList<TResult> Take(Range range)
            {
                int length = range.TryGetOffsetAndLength(count, out int offset);
                return length == 0 ? new EmptyIndexer<TResult>()
                    : length == count ? this
                    : new CreateOffsetIndexer<TResult>(offset, length, selector);
            }
        }
#endif

        private sealed partial class CreateOffsetIndexer<TResult>(int offset, int count, Func<int, TResult> selector) : RangeOffsetIndexerBase<TResult>(offset, count), ISelectWithIndex<TResult>, ISkipBoth<TResult>, ITake<TResult>, ITakeLast<TResult>
        {
            public override TResult this[int index] =>
                index >= 0 && (uint)index < (uint)count
                    ? selector(offset + index)
                    : throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

            public IReadOnlyList<TResult2> Select<TResult2>(Func<TResult, TResult2> _selector) =>
                new CreateOffsetIndexer<TResult2>(offset, count, selector.CombineSelectors(_selector));

            public IReadOnlyList<TResult2> Select<TResult2>(Func<TResult, int, TResult2> _selector) =>
                new CreateOffsetIndexer<TResult2>(offset, count, selector.CombineSelectors(_selector, offset));

            public IReadOnlyList<TResult> Skip(int _count) =>
                _count <= 0 ? this
                    : (uint)_count >= (uint)count ? new EmptyIndexer<TResult>()
                    : new CreateOffsetIndexer<TResult>(offset + _count, count - _count, selector);

            public IReadOnlyList<TResult> SkipLast(int _count) =>
                _count <= 0 ? this
                    : (uint)_count >= (uint)count ? new EmptyIndexer<TResult>()
                    : new CreateOffsetIndexer<TResult>(offset, count - _count, selector);

            public IReadOnlyList<TResult> Take(int _count) =>
                _count <= 0 ? new EmptyIndexer<TResult>()
                    : (uint)_count >= (uint)count ? this
                    : new CreateOffsetIndexer<TResult>(offset, _count, selector);

            public IReadOnlyList<TResult> TakeLast(int _count) =>
                _count <= 0 ? new EmptyIndexer<TResult>()
                    : (uint)_count >= (uint)count ? this
                    : new CreateOffsetIndexer<TResult>(offset + (count - _count), _count, selector);

            public override bool Contains(TResult item)
            {
                int i = count;
                EqualityComparer<TResult> equalityComparer = EqualityComparer<TResult>.Default;
                for (int current = offset; i > 0; i--)
                {
                    if (equalityComparer.Equals(selector(current++), item))
                    {
                        return true;
                    }
                }
                return false;
            }

            public override int IndexOf(TResult item)
            {
                int i = count;
                EqualityComparer<TResult> equalityComparer = EqualityComparer<TResult>.Default;
                for (int current = offset; i > 0; i--)
                {
                    if (equalityComparer.Equals(selector(current++), item))
                    {
                        return count - i;
                    }
                }
                return -1;
            }

            public override void CopyTo(TResult[] array, int arrayIndex)
            {
                int i = count;
                for (int current = offset; i > 0; i--)
                {
                    array[arrayIndex++] = selector(current++);
                }
            }

            public sealed override IEnumerator<TResult> GetEnumerator() => new IndexerEnumerator(offset, count, selector);

            private sealed partial class IndexerEnumerator(int offset, int count, Func<int, TResult> selector) : RangeOffsetIndexerEnumerator(offset, count)
            {
                public override TResult Current => selector(current);
            }
        }

#if COMP_NETSTANDARD2_1
        private sealed partial class CreateOffsetIndexer<TResult> : ITakeRange<TResult>
        {
            public IReadOnlyList<TResult> Take(Range range)
            {
                int length = range.TryGetOffsetAndLength(count, out int offset);
                return length == 0 ? new EmptyIndexer<TResult>()
                    : length == count ? this
                    : new CreateOffsetIndexer<TResult>(this.offset + offset, length, selector);
            }
        }
#endif
    }
}
