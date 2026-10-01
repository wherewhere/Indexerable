using System;
using System.Collections.Generic;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        /// <summary>
        /// Applies a specified function to the corresponding elements of two read-only lists, producing a list of the results.
        /// </summary>
        /// <typeparam name="TFirst">The type of the elements of the first list.</typeparam>
        /// <typeparam name="TSecond">The type of the elements of the second list.</typeparam>
        /// <typeparam name="TResult">The type of the result returned by <paramref name="resultSelector"/>.</typeparam>
        /// <param name="first">The first list to merge.</param>
        /// <param name="second">The second list to merge.</param>
        /// <param name="resultSelector">A function that specifies how to merge the corresponding elements from the two lists.</param>
        /// <returns>A read-only list of the results of applying the specified function to the corresponding elements of the two lists. The result ends when either list ends.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="first"/>, <paramref name="second"/>, or <paramref name="resultSelector"/> is <see langword="null"/>.</exception>
        public static IReadOnlyList<TResult> Zip<TFirst, TSecond, TResult>(this IReadOnlyList<TFirst> first, IReadOnlyList<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
        {
            if (first is null) { ThrowHelper.ThrowArgumentNullException(nameof(first)); }
            if (second is null) { ThrowHelper.ThrowArgumentNullException(nameof(second)); }
            if (resultSelector is null) { ThrowHelper.ThrowArgumentNullException(nameof(resultSelector)); }
            return new ZipIndexer<TFirst, TSecond, TResult>(first, second, resultSelector);
        }

        private sealed partial class ZipIndexer<TFirst, TSecond, TResult>(IReadOnlyList<TFirst> first, IReadOnlyList<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector) : Indexer<TResult>
        {
            public override TResult this[int index] => resultSelector(first[index], second[index]);
            public override int Count => Math.Min(first.Count, second.Count);
        }

#if HAS_VALUETUPLE
        /// <summary>
        /// Produces a list of tuples with elements from two read-only lists.
        /// </summary>
        /// <typeparam name="TFirst">The type of the elements of the first list.</typeparam>
        /// <typeparam name="TSecond">The type of the elements of the second list.</typeparam>
        /// <param name="first">The first list to merge.</param>
        /// <param name="second">The second list to merge.</param>
        /// <returns>A read-only list of tuples containing corresponding elements from the two lists. The result ends when either list ends.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="first"/> or <paramref name="second"/> is <see langword="null"/>.</exception>
        public static IReadOnlyList<(TFirst First, TSecond Second)> Zip<TFirst, TSecond>(this IReadOnlyList<TFirst> first, IReadOnlyList<TSecond> second)
        {
            if (first is null) { ThrowHelper.ThrowArgumentNullException(nameof(first)); }
            if (second is null) { ThrowHelper.ThrowArgumentNullException(nameof(second)); }
            return new ZipToTupleIndexer<TFirst, TSecond>(first, second);
        }

        private sealed partial class ZipToTupleIndexer<TFirst, TSecond>(IReadOnlyList<TFirst> first, IReadOnlyList<TSecond> second) : Indexer<(TFirst First, TSecond Second)>
        {
            public override (TFirst First, TSecond Second) this[int index] => (first[index], second[index]);
            public override int Count => Math.Min(first.Count, second.Count);
        }

        /// <summary>
        /// Produces a list of tuples with elements from three read-only lists.
        /// </summary>
        /// <typeparam name="TFirst">The type of the elements of the first list.</typeparam>
        /// <typeparam name="TSecond">The type of the elements of the second list.</typeparam>
        /// <typeparam name="TThird">The type of the elements of the third list.</typeparam>
        /// <param name="first">The first list to merge.</param>
        /// <param name="second">The second list to merge.</param>
        /// <param name="third">The third list to merge.</param>
        /// <returns>A read-only list of tuples containing corresponding elements from the three lists. The result ends when any list ends.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="first"/>, <paramref name="second"/>, or <paramref name="third"/> is <see langword="null"/>.</exception>
        public static IReadOnlyList<(TFirst First, TSecond Second, TThird Third)> Zip<TFirst, TSecond, TThird>(this IReadOnlyList<TFirst> first, IReadOnlyList<TSecond> second, IReadOnlyList<TThird> third)
        {
            if (first is null) { ThrowHelper.ThrowArgumentNullException(nameof(first)); }
            if (second is null) { ThrowHelper.ThrowArgumentNullException(nameof(second)); }
            if (third is null) { ThrowHelper.ThrowArgumentNullException(nameof(third)); }
            return new ZipToTupleIndexer<TFirst, TSecond, TThird>(first, second, third);
        }

        private sealed partial class ZipToTupleIndexer<TFirst, TSecond, TThird>(IReadOnlyList<TFirst> first, IReadOnlyList<TSecond> second, IReadOnlyList<TThird> third) : Indexer<(TFirst First, TSecond Second, TThird Third)>
        {
            public override (TFirst First, TSecond Second, TThird Third) this[int index] => (first[index], second[index], third[index]);
            public override int Count => Math.Min(first.Count, Math.Min(second.Count, third.Count));
        }
#endif
    }
}
