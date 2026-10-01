using System;
using System.Collections;
using System.Collections.Generic;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        public static IReadOnlyList<TSource> Skip<TSource>(this IReadOnlyList<TSource> source, int count)
        {
            ArgumentNullException.ThrowIfNull(source);
            return count <= 0 ? source
                : source is ISkip<TSource> skip ? skip.Skip(count)
                : new SkipIndexer<TSource>(source, count);
        }

        private interface ISkip<TSource> : IReadOnlyList<TSource>
        {
            IReadOnlyList<TSource> Skip(int count);
        }

        private sealed partial class SkipIndexer<TSource>(IReadOnlyList<TSource> source, int count) : ISkip<TSource>
        {
            public TSource this[int index] =>
                index >= 0 && index < Count
                    ? source[index + count]
                    : throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

            public int Count => Math.Max(0, source.Count - count);

            public IReadOnlyList<TSource> Skip(int _count) => _count <= 0 ? this : new SkipIndexer<TSource>(source, count + _count);

            public IEnumerator<TSource> GetEnumerator()
            {
                for (int i = 0; i < Count; i++)
                {
                    yield return source[i + count];
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public static IReadOnlyList<TSource> SkipLast<TSource>(this IReadOnlyList<TSource> source, int count)
        {
            ArgumentNullException.ThrowIfNull(source);
            return count <= 0 ? source
                : source is ISkipLast<TSource> skip ? skip.SkipLast(count)
                : new SkipLastIndexer<TSource>(source, count);
        }

        private interface ISkipLast<TSource> : IReadOnlyList<TSource>
        {
            IReadOnlyList<TSource> SkipLast(int count);
        }

        private sealed partial class SkipLastIndexer<TSource>(IReadOnlyList<TSource> source, int count) : ISkipLast<TSource>
        {
            public TSource this[int index] =>
                index >= 0 && index < Count
                        ? source[index]
                        : throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

            public int Count => Math.Max(0, source.Count - count);

            public IReadOnlyList<TSource> SkipLast(int _count) => _count <= 0 ? this : new SkipLastIndexer<TSource>(source, count + _count);

            public IEnumerator<TSource> GetEnumerator()
            {
                for (int i = 0; i < Count; i++)
                {
                    yield return source[i];
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
