using System;
using System.Collections;
using System.Collections.Generic;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        public static IReadOnlyList<TSource> Take<TSource>(this IReadOnlyList<TSource> source, int count)
        {
            ArgumentNullException.ThrowIfNull(source);
            return count <= 0 ? (TSource[])[]
                : source is ITake<TSource> take ? take.Take(count)
                : new TakeIndexer<TSource>(source, count);
        }

        private interface ITake<TSource> : IReadOnlyList<TSource>
        {
            IReadOnlyList<TSource> Take(int count);
        }

        private sealed partial class TakeIndexer<TSource>(IReadOnlyList<TSource> source, int count) : ITake<TSource>
        {
            public TSource this[int index] =>
                index >= 0 && index < Count
                    ? source[index]
                    : throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

            public int Count => Math.Min(count, source.Count);

            public IReadOnlyList<TSource> Take(int _count) => _count <= 0 ? (TSource[])[] : new TakeIndexer<TSource>(source, Math.Min(_count, count));

            public IEnumerator<TSource> GetEnumerator()
            {
                for (int i = 0; i < Count; i++)
                {
                    yield return source[i];
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        /// <summary>Returns a specified range of contiguous elements from a sequence.</summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source">The sequence to return elements from.</param>
        /// <param name="range">The range of elements to return, which has start and end indexes either from the start or the end.</param>
        /// <exception cref="ArgumentNullException"><paramref name="source" /> is <see langword="null" />.</exception>
        /// <returns>An <see cref="IReadOnlyList{T}" /> that contains the specified <paramref name="range" /> of elements from the <paramref name="source" /> sequence.</returns>
        /// <remarks>
        /// <para>This method is implemented by using deferred execution. The immediate return value is an object that stores all the information that is required to perform the action. The query represented by this method is not executed until the object is enumerated either by calling its `GetEnumerator` method directly or by using `foreach` in Visual C# or `For Each` in Visual Basic.</para>
        /// <para><see cref="O:Enumerable.Take" /> enumerates <paramref name="source" /> and yields elements whose indices belong to the specified <paramref name="range"/>.</para>
        /// </remarks>
        public static IReadOnlyList<TSource> Take<TSource>(this IReadOnlyList<TSource> source, Range range)
        {
            ArgumentNullException.ThrowIfNull(source);
            return new TakeRangeIndexer<TSource>(source, range);
        }

        private sealed partial class TakeRangeIndexer<TSource>(IReadOnlyList<TSource> source, Range range) : IReadOnlyList<TSource>
        {
            private int GetCount(out int offset)
            {
                int sourceCount = source.Count;
                offset = Math.Max(0, range.Start.GetOffset(sourceCount));
                int end = Math.Min(sourceCount, range.End.GetOffset(sourceCount));
                return end > offset ? end - offset : 0;
            }

            public TSource this[int index] =>
                index >= 0 && index < GetCount(out int offset)
                    ? source[offset + index]
                    : throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

            public int Count => GetCount(out _);

            public IEnumerator<TSource> GetEnumerator()
            {
                int count = GetCount(out int offset);
                for (int i = 0; i < count; i++)
                {
                    yield return source[offset + i];
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public static IReadOnlyList<TSource> TakeLast<TSource>(this IReadOnlyList<TSource> source, int count)
        {
            ArgumentNullException.ThrowIfNull(source);
            return count <= 0 ? (TSource[])[]
                : source is ITakeLast<TSource> take ? take.TakeLast(count)
                : new TakeLastIndexer<TSource>(source, count);
        }

        private interface ITakeLast<TSource> : IReadOnlyList<TSource>
        {
            IReadOnlyList<TSource> TakeLast(int count);
        }

        private sealed partial class TakeLastIndexer<TSource>(IReadOnlyList<TSource> source, int count) : ITakeLast<TSource>
        {
            public TSource this[int index]
            {
                get
                {
                    int takeCount;
                    return index >= 0 && index < (takeCount = Count)
                        ? source[source.Count - takeCount + index]
                        : throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
                }
            }

            public int Count => Math.Min(count, source.Count);

            public IReadOnlyList<TSource> TakeLast(int _count) => _count <= 0 ? (TSource[])[] : new TakeLastIndexer<TSource>(source, Math.Min(_count, count));

            public IEnumerator<TSource> GetEnumerator()
            {
                int takeCount;
                for (int i = 0; i < (takeCount = Count); i++)
                {
                    yield return source[source.Count - takeCount + i];
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
