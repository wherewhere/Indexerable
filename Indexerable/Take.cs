using System;
using System.Collections.Generic;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        /// <summary>
        /// Returns a specified number of contiguous elements from the start of a read-only list.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The list to return elements from.</param>
        /// <param name="count">The number of elements to return.</param>
        /// <returns>A read-only list that contains up to <paramref name="count"/> elements from the start of <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <remarks>If <paramref name="count"/> is less than or equal to zero, the result is empty.</remarks>
        public static IReadOnlyList<TSource> Take<TSource>(this IReadOnlyList<TSource> source, int count)
        {
            if (source is null) { ThrowHelper.ThrowArgumentNullException(nameof(source)); }
            return count <= 0 ? new EmptyIndexer<TSource>()
                : source is ITake<TSource> take ? take.Take(count)
                : new TakeIndexer<TSource>(source, count);
        }

        private interface ITake<out TSource> : IReadOnlyList<TSource>
        {
            /// <inheritdoc cref="Take{TSource}(IReadOnlyList{TSource}, int)"/>
            IReadOnlyList<TSource> Take(int count);
        }

        private sealed partial class TakeIndexer<TSource>(IReadOnlyList<TSource> source, int count) : TakeIndexerBase<TSource>(source), ITake<TSource>
        {
            public override int Count => Math.Min(count, source.Count);
            public IReadOnlyList<TSource> Take(int _count) => _count <= 0 ? new EmptyIndexer<TSource>() : new TakeIndexer<TSource>(source, Math.Min(_count, count));
        }

#if COMP_NETSTANDARD2_1
        /// <summary>
        /// Returns a specified range of contiguous elements from a read-only list.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source">The list to return elements from.</param>
        /// <param name="range">The range of elements to return, which has start and end indexes either from the start or the end.</param>
        /// <exception cref="ArgumentNullException"><paramref name="source" /> is <see langword="null" />.</exception>
        /// <returns>A read-only list that contains the specified <paramref name="range"/> of elements from <paramref name="source"/>.</returns>
        /// <remarks>The returned list is a view over <paramref name="source"/>; elements are accessed as needed and are not copied.</remarks>
        public static IReadOnlyList<TSource> Take<TSource>(this IReadOnlyList<TSource> source, Range range)
        {
            if (source is null) { ThrowHelper.ThrowArgumentNullException(nameof(source)); }
            return source is ITakeRange<TSource> take ? take.Take(range) : new TakeRangeIndexer<TSource>(source, range);
        }

        private interface ITakeRange<out TSource> : IReadOnlyList<TSource>
        {
            /// <inheritdoc cref="Take(System.Range)"/>
            IReadOnlyList<TSource> this[Range range] => Take(range);

            /// <inheritdoc cref="Take{TSource}(IReadOnlyList{TSource}, System.Range)"/>
            IReadOnlyList<TSource> Take(Range range);
        }

        /// <summary>
        /// A extension for <see cref="IReadOnlyList{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source">The list to return elements from.</param>
        extension<TSource>(IReadOnlyList<TSource> source)
        {
            /// <inheritdoc cref="Take{TSource}(IReadOnlyList{TSource}, System.Range)"/>
            public IReadOnlyList<TSource> this[Range range] => source.Take(range);
        }

        private sealed partial class TakeRangeIndexer<TSource>(IReadOnlyList<TSource> source, Range range) : SkipIndexerBase<TSource>(source)
        {
            public override int Count => GetCount(out _);
            protected override int GetCount(out int offset) => range.TryGetOffsetAndLength(source.Count, out offset);
        }

        private static int TryGetOffsetAndLength(this in Range range, int length, out int offset)
        {
            offset = Math.Max(0, range.Start.GetOffset(length));
            int end = Math.Min(length, range.End.GetOffset(length));
            return end > offset ? end - offset : 0;
        }
#endif

        /// <summary>
        /// Returns a specified number of elements from the end of a read-only list.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The list to return elements from.</param>
        /// <param name="count">The number of elements to take from the end of the list.</param>
        /// <returns>A read-only list that contains up to <paramref name="count"/> elements from the end of <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <remarks>If <paramref name="count"/> is less than or equal to zero, the result is empty.</remarks>
        public static IReadOnlyList<TSource> TakeLast<TSource>(this IReadOnlyList<TSource> source, int count)
        {
            if (source is null) { ThrowHelper.ThrowArgumentNullException(nameof(source)); }
            return count <= 0 ? new EmptyIndexer<TSource>()
                : source is ITakeLast<TSource> take ? take.TakeLast(count)
                : new TakeLastIndexer<TSource>(source, count);
        }

        private interface ITakeLast<out TSource> : IReadOnlyList<TSource>
        {
            /// <inheritdoc cref="TakeLast{TSource}(IReadOnlyList{TSource}, int)"/>
            IReadOnlyList<TSource> TakeLast(int count);
        }

        private sealed partial class TakeLastIndexer<TSource>(IReadOnlyList<TSource> source, int count) : SkipIndexerBase<TSource>(source), ITakeLast<TSource>
        {
            public override int Count => Math.Min(count, source.Count);
            public IReadOnlyList<TSource> TakeLast(int _count) => _count <= 0 ? new EmptyIndexer<TSource>() : new TakeLastIndexer<TSource>(source, Math.Min(_count, count));
            protected override int GetCount(out int offset)
            {
                int sourceCount = source.Count, takeCount = Math.Min(count, sourceCount);
                offset = sourceCount - takeCount;
                return takeCount;
            }
        }
    }
}
