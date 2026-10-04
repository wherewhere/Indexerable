using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        /// <summary>
        /// Casts the elements of a non-generic list to the specified type.
        /// </summary>
        /// <typeparam name="TResult">The type to cast the elements of <paramref name="source"/> to.</typeparam>
        /// <param name="source">The non-generic list whose elements are cast.</param>
        /// <returns>A read-only list that contains each element of <paramref name="source"/> cast to <typeparamref name="TResult"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="System.InvalidCastException">An element cannot be cast to <typeparamref name="TResult"/>.</exception>
        public static IReadOnlyList<TResult> Cast<TResult>(this IList source)
        {
            if (source is IReadOnlyList<TResult> typedSource) { return typedSource; }
            else if (source is null) { ThrowHelper.ThrowArgumentNullException(nameof(source)); }
            return source is Array { Rank: > 1 } array ? new ArrayCastIndexer<TResult>(array) : new CastIndexer<TResult>(source);
        }

        private sealed partial class CastIndexer<TResult>(IList source) : Indexer<TResult>
        {
            public override TResult this[int index] => (TResult)source[index]!;
            public override int Count => source.Count;
        }

        /// <summary>
        /// Casts the elements of a read-only list to the specified type.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type to cast the elements of <paramref name="source"/> to.</typeparam>
        /// <param name="source">The read-only list whose elements are cast.</param>
        /// <returns>A read-only list that contains each element of <paramref name="source"/> cast to <typeparamref name="TResult"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidCastException">An element cannot be cast to <typeparamref name="TResult"/>.</exception>
        public static IReadOnlyList<TResult> Cast<TSource, TResult>(this IReadOnlyList<TSource> source)
        {
            if (source is IReadOnlyList<TResult> typedSource) { return typedSource; }
            else if (source is null) { ThrowHelper.ThrowArgumentNullException(nameof(source)); }
            return new CastIndexer<TSource, TResult>(source);
        }

        private sealed partial class CastIndexer<TSource, TResult>(IReadOnlyList<TSource> source) : Indexer<TResult>
        {
            public override TResult this[int index] => (TResult)(object)source[index]!;
            public override int Count => source.Count;
        }

        private sealed partial class ArrayCastIndexer<TResult>(Array source) : IReadOnlyList<TResult>
        {
            public TResult this[int index]
            {
                get
                {
                    if (index < 0 || (uint)index >= (uint)source.Length)
                    {
                        ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLess(nameof(index));
                    }

                    switch (source.Rank)
                    {
                        case 1:
                            return (TResult)source.GetValue(source.GetLowerBound(0) + index)!;
                        case 2:
                            int length1 = source.GetLength(1);
                            return (TResult)source.GetValue(
                                source.GetLowerBound(0) + (index / length1),
                                source.GetLowerBound(1) + (index % length1))!;
                        case 3:
                            length1 = source.GetLength(1);
                            int length2 = source.GetLength(2);
                            return (TResult)source.GetValue(
                                source.GetLowerBound(0) + (index / (length1 * length2)),
                                source.GetLowerBound(1) + (index / length2 % length1),
                                source.GetLowerBound(2) + (index % length2))!;
                        default:
                            int rank = source.Rank;
                            int[] indices = new int[rank];
                            for (int i = rank - 1; i >= 0; i--)
                            {
                                int length = source.GetLength(i);
                                indices[i] = source.GetLowerBound(i) + (index % length);
                                index /= length;
                            }
                            return (TResult)source.GetValue(indices)!;
                    }
                }
            }

            public int Count => source.Length;

            public IEnumerator<TResult> GetEnumerator() => Enumerable.Cast<TResult>(source).GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
