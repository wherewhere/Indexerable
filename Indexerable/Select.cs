using System;
using System.Collections.Generic;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        public static IReadOnlyList<TResult> Select<TSource, TResult>(this IReadOnlyList<TSource> source, Func<TSource, TResult> selector)
        {
            if (source is null) { ThrowHelper.ThrowArgumentNullException(nameof(source)); }
            if (selector is null) { ThrowHelper.ThrowArgumentNullException(nameof(selector)); }
            return source is ISelect<TSource> select
                ? select.Select(selector) : new IReadOnlyListSelectIndexer<TSource, TResult>(source, selector);
        }

        private interface ISelect<TSource> : IReadOnlyList<TSource>
        {
            IReadOnlyList<TResult> Select<TResult>(Func<TSource, TResult> selector);
        }

        /// <summary>
        /// An iterator that maps each item of an <see cref="IReadOnlyList{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the source enumerable.</typeparam>
        /// <typeparam name="TResult">The type of the mapped items.</typeparam>
        private sealed partial class IReadOnlyListSelectIndexer<TSource, TResult>(IReadOnlyList<TSource> source, Func<TSource, TResult> selector) : Indexer<TResult>, ISelect<TResult>
        {
            public override TResult this[int index] => selector(source[index]);
            public override int Count => source.Count;
            public IReadOnlyList<TResult2> Select<TResult2>(Func<TResult, TResult2> _selector) =>
                new IReadOnlyListSelectIndexer<TSource, TResult2>(source, selector.CombineSelectors(_selector));
        }

        public static IReadOnlyList<TResult> Select<TSource, TResult>(this IReadOnlyList<TSource> source, Func<TSource, int, TResult> selector)
        {
            if (source is null) { ThrowHelper.ThrowArgumentNullException(nameof(source)); }
            if (selector is null) { ThrowHelper.ThrowArgumentNullException(nameof(selector)); }
            return source is ISelectWithIndex<TSource> select
                ? select.Select(selector) : new IReadOnlyListSelectWithIndexIndexer<TSource, TResult>(source, selector);
        }

        private interface ISelectWithIndex<TSource> : ISelect<TSource>
        {
            IReadOnlyList<TResult> Select<TResult>(Func<TSource, int, TResult> selector);
        }

        private sealed partial class IReadOnlyListSelectWithIndexIndexer<TSource, TResult>(IReadOnlyList<TSource> source, Func<TSource, int, TResult> selector) : Indexer<TResult>, ISelectWithIndex<TResult>
        {
            public override TResult this[int index] => selector(source[index], index);
            public override int Count => source.Count;
            public IReadOnlyList<TResult2> Select<TResult2>(Func<TResult, TResult2> _selector) =>
                new IReadOnlyListSelectWithIndexIndexer<TSource, TResult2>(source, selector.CombineSelectors(_selector));
            public IReadOnlyList<TResult2> Select<TResult2>(Func<TResult, int, TResult2> _selector) =>
                new IReadOnlyListSelectWithIndexIndexer<TSource, TResult2>(source, selector.CombineSelectors(_selector));
        }
    }
}
