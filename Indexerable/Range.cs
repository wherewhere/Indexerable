using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        public static IReadOnlyList<int> Range(int start, int count)
        {
            long max = ((long)start) + count - 1;
            if (count < 0 || max > int.MaxValue)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(count));
            }
            return count > 0 ? new RangeIndexer(start, count) : [];
        }

        /// <summary>
        /// An iterator that yields a range of consecutive integers.
        /// </summary>
        [DebuggerDisplay("Count = {Count}")]
        private sealed partial class RangeIndexer(int start, int count) : Indexer<int>
        {
            public override int this[int index] => index >= 0 && index < count ? start + index : throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
            public override int Count => count;
        }
    }
}
