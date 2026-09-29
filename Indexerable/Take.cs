using System;
using System.Collections.Generic;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        public static IReadOnlyList<TSource> Take<TSource>(this IReadOnlyList<TSource> source, int count)
        {
            ArgumentNullException.ThrowIfNull(source);
            return count > 0 ? new TakeIndexer<TSource>(source, count) : [];
        }

        private sealed partial class TakeIndexer<TSource>(IReadOnlyList<TSource> source, int count) : Indexer<TSource>
        {
            public override TSource this[int index] =>
                index >= 0 && index < Count
                    ? source[index] 
                    : throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
            public override int Count => Math.Min(count, source.Count);
        }

        /// <summary>Returns a specified range of contiguous elements from a sequence.</summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source">The sequence to return elements from.</param>
        /// <param name="range">The range of elements to return, which has start and end indexes either from the start or the end.</param>
        /// <exception cref="ArgumentNullException"><paramref name="source" /> is <see langword="null" />.</exception>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the specified <paramref name="range" /> of elements from the <paramref name="source" /> sequence.</returns>
        /// <remarks>
        /// <para>This method is implemented by using deferred execution. The immediate return value is an object that stores all the information that is required to perform the action. The query represented by this method is not executed until the object is enumerated either by calling its `GetEnumerator` method directly or by using `foreach` in Visual C# or `For Each` in Visual Basic.</para>
        /// <para><see cref="O:Enumerable.Take" /> enumerates <paramref name="source" /> and yields elements whose indices belong to the specified <paramref name="range"/>.</para>
        /// </remarks>
        public static IReadOnlyList<TSource> Take<TSource>(this IReadOnlyList<TSource> source, Range range)
        {
            ArgumentNullException.ThrowIfNull(source);
            return new RangeIndexer<TSource>(source, range);
        }

        private sealed partial class RangeIndexer<TSource>(IReadOnlyList<TSource> source, Range range) : Indexer<TSource>
        {
            private int GetCount(out int offset)
            {
                int sourceCount = source.Count;
                offset = Math.Max(0, range.Start.GetOffset(sourceCount));
                int end = Math.Min(sourceCount, range.End.GetOffset(sourceCount));
                return end > offset ? end - offset : 0;
            }

            public override TSource this[int index] =>
                index >= 0 && index < GetCount(out int offset)
                    ? source[offset + index]
                    : throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

            public override int Count => GetCount(out _);
        }
    }
}
