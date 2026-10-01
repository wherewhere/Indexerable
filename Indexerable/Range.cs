using System;
using System.Collections;
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
            return count > 0 ? new RangeIndexer(start, count) : (int[])[];
        }

        /// <summary>
        /// An iterator that yields a range of consecutive integers.
        /// </summary>
        [DebuggerDisplay("Count = {Count}")]
        private sealed partial class RangeIndexer(int start, int count) : ITake<int>
        {
            public int this[int index] =>
                index >= 0 && index < count
                    ? start + index
                    : throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

            public int Count => count;

            public IReadOnlyList<int> Take(int _count) => _count <= 0 ? (int[])[] : new RangeIndexer(start, Math.Min(_count, count));

            public IEnumerator<int> GetEnumerator()
            {
                int i = count;
                for (int current = start; i > 0; i--)
                {
                    yield return current;
                    current++;
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
