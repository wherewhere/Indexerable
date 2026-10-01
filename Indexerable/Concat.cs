using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        public static IReadOnlyList<TSource> Concat<TSource>(this IReadOnlyList<TSource> first, IReadOnlyList<TSource> second)
        {
            if (first is null) { ThrowHelper.ThrowArgumentNullException(nameof(first)); }
            if (second is null) { ThrowHelper.ThrowArgumentNullException(nameof(second)); }
            return new ConcatIndexer<TSource>(first, second);
        }

        private interface IConcat<TSource> : IReadOnlyList<TSource>
        {
            IReadOnlyList<TSource> Concat(IReadOnlyList<TSource> second);
        }

        /// <summary>
        /// Represents the concatenation of two <see cref="IReadOnlyList{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the source enumerables.</typeparam>
        private sealed partial class ConcatIndexer<TSource>(IReadOnlyList<TSource> first, IReadOnlyList<TSource> second) : IConcat<TSource>
        {
            public TSource this[int index]
            {
                get
                {
                    int firstCount;
                    return index < (firstCount = first.Count) ? first[index] : second[index - firstCount];
                }
            }

            public int Count => first.Count + second.Count;

            public IReadOnlyList<TSource> Concat(IReadOnlyList<TSource> _second) => new ConcatNIndexer<TSource>(first, second, _second);

            public IEnumerator<TSource> GetEnumerator()
            {
                foreach (TSource? item in first)
                {
                    yield return item;
                }
                foreach (TSource? item in second)
                {
                    yield return item;
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        /// <summary>
        /// Represents the concatenation of three or more <see cref="IReadOnlyList{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the source enumerables.</typeparam>
        /// <remarks>
        /// To handle chains of >= 3 sources, we chain the <see cref="Concat"/> iterators together and allow
        /// <see cref="GetEnumerable"/> to fetch enumerables from the previous sources.  This means that rather
        /// than each <see cref="IEnumerator.MoveNext"/> and <see cref="IEnumerator{T}.Current"/> calls having to traverse all of the previous
        /// sources, we only have to traverse all of the previous sources once per chained enumerable.  An alternative
        /// would be to use an array to store all of the enumerables, but this has a much better memory profile and
        /// without much additional run-time cost.
        /// </remarks>
        private sealed partial class ConcatNIndexer<TSource>(IReadOnlyList<TSource> first, params IReadOnlyList<TSource>[] rest) : IConcat<TSource>
        {
            public TSource this[int index]
            {
                get
                {
                    int offset;
                    if (index < (offset = first.Count))
                    {
                        return first[index];
                    }
                    int temp = offset;
                    foreach (IReadOnlyList<TSource> source in rest)
                    {
                        if (index < (temp += source.Count))
                        {
                            return source[index - offset];
                        }
                        offset = temp;
                    }
                    throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
                }
            }

            public int Count => checked(first.Count + rest.Sum(source => source.Count));

            public IReadOnlyList<TSource> Concat(IReadOnlyList<TSource> _second) => new ConcatNIndexer<TSource>(first, [.. rest, _second]);

            public IEnumerator<TSource> GetEnumerator()
            {
                foreach (TSource item in first)
                {
                    yield return item;
                }
                foreach (IReadOnlyList<TSource> source in rest)
                {
                    foreach (TSource item in source)
                    {
                        yield return item;
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
