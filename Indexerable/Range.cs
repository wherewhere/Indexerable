using System;
using System.Collections.Generic;
using System.Diagnostics;

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
            return count > 0 ? new RangeIndexer(start, count) : (int[])[];
        }

        /// <summary>
        /// An iterator that yields a range of consecutive integers.
        /// </summary>
        [DebuggerDisplay("Count = {Count}")]
        private sealed partial class RangeIndexer(int start, int count) : IndexerBase<int>, ITake<int>
        {
            public override int this[int index] =>
                index >= 0 && index < count
                    ? start + index
                    : throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

            public override int Count => count;

            public IReadOnlyList<int> Take(int _count) => _count <= 0 ? (int[])[] : new RangeIndexer(start, Math.Min(_count, count));

            public override bool Contains(int item) => item >= start && item < start + count;

            public override int IndexOf(int item)
            {
                int sub = item - start;
                return sub >= 0 && sub < count ? sub : -1;
            }

            public override void CopyTo(int[] array, int arrayIndex)
            {
                int i = count;
                for (int current = start; i > 0; i--)
                {
                    array[arrayIndex++] = current;
                    current++;
                }
            }

            public override IEnumerator<int> GetEnumerator()
            {
                int i = count;
                for (int current = start; i > 0; i--)
                {
                    yield return current;
                    current++;
                }
            }
        }
    }
}
