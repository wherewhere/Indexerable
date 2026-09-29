using System;
using System.Collections.Generic;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        public static IReadOnlyList<TResult> Zip<TFirst, TSecond, TResult>(this IReadOnlyList<TFirst> first, IReadOnlyList<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
        {
            ArgumentNullException.ThrowIfNull(first);
            ArgumentNullException.ThrowIfNull(second);
            ArgumentNullException.ThrowIfNull(resultSelector);
            return new ZipIndexer<TFirst, TSecond, TResult>(first, second, resultSelector);
        }

        private sealed partial class ZipIndexer<TFirst, TSecond, TResult>(IReadOnlyList<TFirst> first, IReadOnlyList<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector) : Indexer<TResult>
        {
            public override TResult this[int index] => resultSelector(first[index], second[index]);
            public override int Count => Math.Min(first.Count, second.Count);
        }

        public static IReadOnlyList<(TFirst First, TSecond Second)> Zip<TFirst, TSecond>(this IReadOnlyList<TFirst> first, IReadOnlyList<TSecond> second)
        {
            ArgumentNullException.ThrowIfNull(first);
            ArgumentNullException.ThrowIfNull(second);
            return new ZipToTupleIndexer<TFirst, TSecond>(first, second);
        }

        private sealed partial class ZipToTupleIndexer<TFirst, TSecond>(IReadOnlyList<TFirst> first, IReadOnlyList<TSecond> second) : Indexer<(TFirst First, TSecond Second)>
        {
            public override (TFirst First, TSecond Second) this[int index] => (first[index], second[index]);
            public override int Count => Math.Min(first.Count, second.Count);
        }

        /// <summary>
        /// Produces a sequence of tuples with elements from the three specified sequences.
        /// </summary>
        /// <typeparam name="TFirst">The type of the elements of the first input sequence.</typeparam>
        /// <typeparam name="TSecond">The type of the elements of the second input sequence.</typeparam>
        /// <typeparam name="TThird">The type of the elements of the third input sequence.</typeparam>
        /// <param name="first">The first sequence to merge.</param>
        /// <param name="second">The second sequence to merge.</param>
        /// <param name="third">The third sequence to merge.</param>
        /// <returns>A sequence of tuples with elements taken from the first, second, and third sequences, in that order.</returns>
        public static IReadOnlyList<(TFirst First, TSecond Second, TThird Third)> Zip<TFirst, TSecond, TThird>(this IReadOnlyList<TFirst> first, IReadOnlyList<TSecond> second, IReadOnlyList<TThird> third)
        {
            ArgumentNullException.ThrowIfNull(first);
            ArgumentNullException.ThrowIfNull(second);
            ArgumentNullException.ThrowIfNull(third);
            return new ZipToTupleIndexer<TFirst, TSecond, TThird>(first, second, third);
        }

        private sealed partial class ZipToTupleIndexer<TFirst, TSecond, TThird>(IReadOnlyList<TFirst> first, IReadOnlyList<TSecond> second, IReadOnlyList<TThird> third) : Indexer<(TFirst First, TSecond Second, TThird Third)>
        {
            public override (TFirst First, TSecond Second, TThird Third) this[int index] => (first[index], second[index], third[index]);
            public override int Count => Math.Min(first.Count, Math.Min(second.Count, third.Count));
        }
    }
}
