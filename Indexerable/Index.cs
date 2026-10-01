#if HAS_VALUETUPLE
using System;
using System.Collections.Generic;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        /// <summary>
        /// Projects each element of a read-only list together with its zero-based index.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source">The read-only list whose elements and indexes are returned.</param>
        /// <returns>A read-only list of tuples containing each element's zero-based index and value.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source" /> is <see langword="null" />.</exception>
        public static IReadOnlyList<(int Index, TSource Item)> Index<TSource>(this IReadOnlyList<TSource> source)
        {
            if (source is null) { ThrowHelper.ThrowArgumentNullException(nameof(source)); }
            return new IndexIndexer<TSource>(source);
        }

        private sealed partial class IndexIndexer<TSource>(IReadOnlyList<TSource> source) : Indexer<(int Index, TSource Item)>
        {
            public override (int Index, TSource Item) this[int index] => (index, source[index]);
            public override int Count => source.Count;
        }
    }
}
#endif