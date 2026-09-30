using System;
using System.Collections.Generic;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        public static IReadOnlyList<TSource> Concat<TSource>(this IReadOnlyList<TSource> first, IReadOnlyList<TSource> second)
        {
            ArgumentNullException.ThrowIfNull(first);
            ArgumentNullException.ThrowIfNull(second);
            return new ConcatIndexer<TSource>(first, second);
        }

        /// <summary>
        /// Represents the concatenation of two <see cref="IReadOnlyList{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the source enumerables.</typeparam>
        private sealed partial class ConcatIndexer<TSource>(IReadOnlyList<TSource> first, IReadOnlyList<TSource> second) : Indexer<TSource>
        {
            public override TSource this[int index] => index < first.Count ? first[index] : second[index - first.Count];
            public override int Count => first.Count + second.Count;
        }
    }
}
