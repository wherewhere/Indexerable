using System;
using System.Collections.Generic;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        /// <summary>
        /// Generates a read-only list that contains a repeated value.
        /// </summary>
        /// <typeparam name="TResult">The type of the value to repeat.</typeparam>
        /// <param name="element">The value to repeat in the resulting list.</param>
        /// <param name="count">The number of times to repeat the value.</param>
        /// <returns>A read-only list that contains <paramref name="element"/> repeated <paramref name="count"/> times.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is less than zero.</exception>
        public static IReadOnlyList<TResult> Repeat<TResult>(TResult element, int count)
        {
            if (count < 0) { ThrowHelper.ThrowArgumentOutOfRangeException(nameof(count)); }
            return count switch
            {
                0 => new EmptyIndexer<TResult>(),
                1 => new SingleElementIndexer<TResult>(element),
                _ => new RepeatIndexer<TResult>(element, count),
            };
        }

        /// <summary>
        /// An iterator that yields the same item multiple times.
        /// </summary>
        /// <typeparam name="TResult">The type of the item.</typeparam>
        private sealed partial class RepeatIndexer<TResult>(TResult element, int count) : RangeIndexerBase<TResult>(count), ISkipBoth<TResult>, ITake<TResult>, ITakeLast<TResult>
        {
            public override TResult this[int index] => index >= 0 && (uint)index < (uint)count ? element : throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
            public IReadOnlyList<TResult> Skip(int _count) => _count <= 0 ? this : (uint)_count >= (uint)count ? new EmptyIndexer<TResult>() : new RepeatIndexer<TResult>(element, count - _count);
            public IReadOnlyList<TResult> SkipLast(int _count) => Skip(_count);
            public IReadOnlyList<TResult> Take(int _count) => _count <= 0 ? new EmptyIndexer<TResult>() : new RepeatIndexer<TResult>(element, Math.Min(_count, count));
            public IReadOnlyList<TResult> TakeLast(int _count) => Take(_count);
            public override bool Contains(TResult item) => EqualityComparer<TResult>.Default.Equals(element, item);
            public override int IndexOf(TResult item) => EqualityComparer<TResult>.Default.Equals(element, item) ? 0 : -1;
            public override void CopyTo(TResult[] array, int arrayIndex) => Array.Fill(array, element, arrayIndex, count);
            public override IEnumerator<TResult> GetEnumerator() => new IndexerEnumerator(element, count);
            private sealed class IndexerEnumerator(TResult element, int count) : RangeIndexerEnumerator(count)
            {
                public override TResult Current => element;
            }
        }
    }
}
