using System;
using System.Collections.Generic;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        /// <summary>
        /// Returns a sequence with the elements of <paramref name="source"/> in reverse order.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The sequence whose elements should be reversed.</param>
        /// <returns>A sequence that enumerates the elements of <paramref name="source"/> in reverse.</returns>
        public static IReadOnlyList<TSource> Reverse<TSource>(this IReadOnlyList<TSource> source)
        {
            ArgumentNullException.ThrowIfNull(source);
            return new ReverseIndexer<TSource>(source);
        }

        /// <summary>
        /// An iterator that yields the items of an <see cref="IReadOnlyList{TSource}"/> in reverse.
        /// </summary>
        /// <typeparam name="TSource">The type of the source enumerable.</typeparam>
        private sealed partial class ReverseIndexer<TSource>(IReadOnlyList<TSource> source) : Indexer<TSource>
        {
            public override TSource this[int index] => source[Count - 1 - index];
            public override int Count => source.Count;
        }
    }
}
