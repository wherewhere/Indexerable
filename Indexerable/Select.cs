using System;
using System.Collections.Generic;
using System.Linq;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        public static IReadOnlyList<TResult> Select<TSource, TResult>(this IReadOnlyList<TSource> source, Func<TSource, TResult> selector)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(selector);
            return new IReadOnlyListSelectIndexer<TSource, TResult>(source, selector);
        }

        /// <summary>
        /// An iterator that maps each item of an <see cref="IEnumerable{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the source enumerable.</typeparam>
        /// <typeparam name="TResult">The type of the mapped items.</typeparam>
        private sealed partial class IReadOnlyListSelectIndexer<TSource, TResult>(IReadOnlyList<TSource> source, Func<TSource, TResult> selector) : Indexer<TResult>
        {
            public override TResult this[int index] => selector(source[index]);
            public override int Count => source.Count;
        }

        public static IReadOnlyList<TResult> Select<TSource, TResult>(this IReadOnlyList<TSource> source, Func<TSource, int, TResult> selector)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(selector);
            return new IReadOnlyListSelectWithIndexIndexer<TSource, TResult>(source, selector);
        }

        private sealed partial class IReadOnlyListSelectWithIndexIndexer<TSource, TResult>(IReadOnlyList<TSource> source, Func<TSource, int, TResult> selector) : Indexer<TResult>
        {
            public override TResult this[int index] => selector(source[index], index);
            public override int Count => source.Count;
        }
    }
}
