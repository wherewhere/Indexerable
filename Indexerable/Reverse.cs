using System;
using System.Collections.Generic;
using System.Linq;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        /// <summary>
        /// Inverts the order of the elements in a read-only list.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The list whose elements should be returned in reverse order.</param>
        /// <returns>A read-only list whose elements correspond to those of <paramref name="source"/> in reverse order.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        public static IReadOnlyList<TSource> Reverse<TSource>(this IReadOnlyList<TSource> source)
        {
            if (source is null) { ThrowHelper.ThrowArgumentNullException(nameof(source)); }
            return new ReverseIndexer<TSource>(source);
        }

        /// <summary>
        /// An iterator that yields the items of an <see cref="IReadOnlyList{TSource}"/> in reverse.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the source list.</typeparam>
        private sealed partial class ReverseIndexer<TSource>(IReadOnlyList<TSource> source) : Indexer<TSource>
        {
            public override TSource this[int index] => source[Count - 1 - index];
            public override int Count => source.Count;
            public override bool Contains(TSource item) => source.Contains(item);
            public override int IndexOf(TSource item)
            {
                int index = source.IndexOf(item);
                return index < 0 ? -1 : Count - 1 - index;
            }
        }
    }
}
