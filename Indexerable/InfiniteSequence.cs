#if NET7_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        /// <summary>
        /// Generates an unbounded numeric sequence that begins with <paramref name="start"/> and increments each subsequent value by <paramref name="step"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value to be yielded in the result sequence.</typeparam>
        /// <param name="start">The starting value.</param>
        /// <param name="step">The amount by which the next yielded value should be incremented from the previous yielded value.</param>
        /// <returns>A read-only list view that reports <see cref="int.MaxValue"/> elements and whose enumerator yields values indefinitely.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="start"/> or <paramref name="step"/> is <see langword="null"/>.</exception>
        public static IReadOnlyList<T> InfiniteSequence<T>(T start, T step) where T : INumberBase<T>
        {
            if (start is null) { ThrowHelper.ThrowArgumentNullException(nameof(start)); }
            if (step is null) { ThrowHelper.ThrowArgumentNullException(nameof(step)); }
            return new InfiniteSequenceIndexer<T>(start, step);
        }

        private sealed partial class InfiniteSequenceIndexer<T>(T start, T step) : IndexerBase<T> where T : INumberBase<T>
        {
            public override T this[int index]
            {
                get
                {
                    if (index < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative.");
                    }
                    else
                    {
#if NET9_0_OR_GREATER
                        T result = T.MultiplyAddEstimate(step, T.CreateTruncating(index), start);
                        if (!T.IsInfinity(result))
                        {
                            return result;
                        }
#else
                        T mult = step * T.CreateTruncating(index);
                        if (!T.IsInfinity(mult))
                        {
                            return start + mult;
                        }
#endif
                        T temp = start;
                        for (int i = 0; i < index; i++)
                        {
                            temp += step;
                        }
                        return temp;
                    }
                }
            }

            public override int Count => int.MaxValue;

            public override bool Contains(T item)
            {
                if (T.IsZero(step))
                {
                    return item == start;
                }
                T sub = item - start;
                return T.IsZero(sub) || ((T.IsPositive(step) ? T.IsPositive(sub) : T.IsNegative(sub)) && ((T.IsInteger(start) && step == T.One && T.IsInteger(item)) || T.IsInteger(sub / step)));
            }

            public override int IndexOf(T item)
            {
                if (T.IsZero(step))
                {
                    return item == start ? 0 : -1;
                }
                T sub = item - start;
                if (T.IsZero(sub))
                {
                    return 0;
                }
                if (T.IsPositive(step) ? T.IsPositive(sub) : T.IsNegative(sub))
                {
                    T index = sub / step;
                    if (T.IsInteger(index))
                    {
                        return int.CreateChecked(index);
                    }
                }
                return -1;
            }

            public override void CopyTo(T[] array, int arrayIndex) => throw new NotSupportedException("Cannot copy an infinite sequence to an array.");

            public override IEnumerator<T> GetEnumerator() => new IndexerEnumerator(start, step);

            private sealed partial class IndexerEnumerator(T start, T step) : IndexerEnumeratorBase
            {
                private readonly T start = start;
                private T current = start;
                public override T Current => current;

                public override bool MoveNext()
                {
                    switch (_index)
                    {
                        case -1:
                            _index = 0;
                            return true;
                        default:
                            current += step;
                            return true;
                    }
                }

                public override void Reset()
                {
                    base.Reset();
                    current = start;
                }
            }
        }
    }
}
#endif