using System;
using System.Collections.Generic;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        public static IReadOnlyList<TSource> Skip<TSource>(this IReadOnlyList<TSource> source, int count)
        {
            ArgumentNullException.ThrowIfNull(source);
            if (count < 0) { count = 0; }
            return new SkipIndexer<TSource>(source, count);
        }

        private sealed partial class SkipIndexer<TSource>(IReadOnlyList<TSource> source, int count) : Indexer<TSource>
        {
            public override TSource this[int index] =>
                index >= 0 && index < Count
                    ? source[index + count]
                    : throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
            public override int Count => Math.Max(0, source.Count - count);
        }
    }
}
