using System;
using System.Collections.Generic;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        /// <summary>Returns an enumerable that incorporates the element's index into a tuple.</summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source">The source enumerable providing the elements.</param>
        /// <returns>An enumerable that incorporates each element index into a tuple.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source" /> is <see langword="null" />.</exception>
        public static IReadOnlyList<(int Index, TSource Item)> Index<TSource>(this IReadOnlyList<TSource> source)
        {
            ArgumentNullException.ThrowIfNull(source);
            return new IndexIndexer<TSource>(source);
        }

        private sealed partial class IndexIndexer<TSource>(IReadOnlyList<TSource> source) : Indexer<(int Index, TSource Item)>
        {
            public override (int Index, TSource Item) this[int index] => (index, source[index]);
            public override int Count => source.Count;
        }
    }
}
