using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        /// <summary>Generates an infinite sequence that begins with <paramref name="start"/> and yields additional values each incremented by <paramref name="step"/>.</summary>
        /// <typeparam name="T">The type of the value to be yielded in the result sequence.</typeparam>
        /// <param name="start">The starting value.</param>
        /// <param name="step">The amount by which the next yielded value should be incremented from the previous yielded value.</param>
        /// <returns>An <see cref="IReadOnlyList{T}"/> that contains the sequence.</returns>
        public static IReadOnlyList<T> InfiniteSequence<T>(T start, T step) where T : INumberBase<T>
        {
            ArgumentNullException.ThrowIfNull(start);
            ArgumentNullException.ThrowIfNull(step);
            return new InfiniteSequenceIndexer<T>(start, step);
        }

        private sealed partial class InfiniteSequenceIndexer<T>(T start, T step) : IReadOnlyList<T> where T : INumberBase<T>
        {
            public T this[int index]
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

            public int Count => int.MaxValue;

            public IEnumerator<T> GetEnumerator()
            {
                T current = start;
                while (true)
                {
                    yield return current;
                    current += step;
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
