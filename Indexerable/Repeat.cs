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
        private sealed partial class RepeatIndexer<TResult>(TResult element, int count) : Indexer<TResult>
        {
            public override TResult this[int index] => index >= 0 && index < count ? element : throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
            public override int Count => count;
        }
    }
}
