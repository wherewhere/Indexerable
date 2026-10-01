using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        public static IReadOnlyList<TResult> Repeat<TResult>(TResult element, int count)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(count);
            return count switch
            {
                0 => (TResult[])[],
                1 => (TResult[])[element],
                _ => new RepeatIndexer<TResult>(element, count),
            };
        }

        /// <summary>
        /// An iterator that yields the same item multiple times.
        /// </summary>
        /// <typeparam name="TResult">The type of the item.</typeparam>
        [DebuggerDisplay("Count = {Count}")]
        private sealed partial class RepeatIndexer<TResult>(TResult element, int count) : Indexer<TResult>, ISkip<TResult>, ISkipLast<TResult>, ITake<TResult>, ITakeLast<TResult>
        {
            public override TResult this[int index] => index >= 0 && index < count ? element : throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
            public override int Count => count;
            public IReadOnlyList<TResult> Skip(int _count) => _count <= 0 ? this : _count >= count ? (TResult[])[] : new RepeatIndexer<TResult>(element, count - _count);
            public IReadOnlyList<TResult> SkipLast(int _count) => Skip(_count);
            public IReadOnlyList<TResult> Take(int _count) => _count <= 0 ? (TResult[])[] : new RepeatIndexer<TResult>(element, Math.Min(_count, count));
            public IReadOnlyList<TResult> TakeLast(int _count) => Take(_count);
        }
    }
}
