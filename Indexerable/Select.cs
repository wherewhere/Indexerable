using System;
using System.Collections.Generic;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        /// <summary>
        /// Projects each element of a read-only list into a new form.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by <paramref name="selector"/>.</typeparam>
        /// <param name="source">A read-only list whose elements are projected.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>A read-only list whose elements are the result of invoking the transform function on each element of <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is <see langword="null"/>.</exception>
        /// <remarks>The selector is invoked when the corresponding element is accessed; the projected values are not materialized when this method is called.</remarks>
        public static IReadOnlyList<TResult> Select<TSource, TResult>(this IReadOnlyList<TSource> source, Func<TSource, TResult> selector)
        {
            if (source is null) { ThrowHelper.ThrowArgumentNullException(nameof(source)); }
            if (selector is null) { ThrowHelper.ThrowArgumentNullException(nameof(selector)); }
            return source is ISelect<TSource> select
                ? select.Select(selector) : new IReadOnlyListSelectIndexer<TSource, TResult>(source, selector);
        }

        private interface ISelect<out TSource> : IReadOnlyList<TSource>
        {
            /// <inheritdoc cref="Select{TSource, TResult}(IReadOnlyList{TSource}, Func{TSource, TResult})"/>
            IReadOnlyList<TResult> Select<TResult>(Func<TSource, TResult> selector);
        }

        /// <summary>
        /// An iterator that maps each item of an <see cref="IReadOnlyList{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the source list.</typeparam>
        /// <typeparam name="TResult">The type of the mapped items.</typeparam>
        private sealed partial class IReadOnlyListSelectIndexer<TSource, TResult>(IReadOnlyList<TSource> source, Func<TSource, TResult> selector) : Indexer<TResult>, ISelect<TResult>
        {
            public override TResult this[int index] => selector(source[index]);
            public override int Count => source.Count;
            public IReadOnlyList<TResult2> Select<TResult2>(Func<TResult, TResult2> _selector) =>
                new IReadOnlyListSelectIndexer<TSource, TResult2>(source, selector.CombineSelectors(_selector));
        }

        /// <summary>
        /// Projects each element of a read-only list into a new form by incorporating the element's index.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by <paramref name="selector"/>.</typeparam>
        /// <param name="source">A read-only list whose elements are projected.</param>
        /// <param name="selector">A transform function to apply to each element; its second parameter is the zero-based index of the element.</param>
        /// <returns>A read-only list whose elements are the result of invoking the indexed transform function on each element of <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is <see langword="null"/>.</exception>
        /// <remarks>The selector is invoked when the corresponding element is accessed; the projected values are not materialized when this method is called.</remarks>
        public static IReadOnlyList<TResult> Select<TSource, TResult>(this IReadOnlyList<TSource> source, Func<TSource, int, TResult> selector)
        {
            if (source is null) { ThrowHelper.ThrowArgumentNullException(nameof(source)); }
            if (selector is null) { ThrowHelper.ThrowArgumentNullException(nameof(selector)); }
            return source is ISelectWithIndex<TSource> select
                ? select.Select(selector) : new IReadOnlyListSelectWithIndexIndexer<TSource, TResult>(source, selector);
        }

        private interface ISelectWithIndex<out TSource> : ISelect<TSource>
        {
            /// <inheritdoc cref="Select{TSource, TResult}(IReadOnlyList{TSource}, Func{TSource, int, TResult})"/>
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
