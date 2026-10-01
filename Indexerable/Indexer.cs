using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        /// <summary>
        /// A base class for read-only lists whose elements are loaded on demand.
        /// </summary>
        /// <typeparam name="TSource">The type of each item to yield.</typeparam>
        /// <remarks>
        /// Each instance represents a fixed list operation. The base implementation creates a separate
        /// enumerator whose position changes as the list is enumerated.
        /// </remarks>
        private abstract partial class Indexer<TSource> : IReadOnlyList<TSource>
        {
            public abstract TSource this[int index] { get; }

            public abstract int Count { get; }

            public virtual IEnumerator<TSource> GetEnumerator() => new IndexerEnumerator(this);

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            private sealed class IndexerEnumerator(Indexer<TSource> source) : IEnumerator<TSource>
            {
                private int _index = -1;

                public TSource Current => source[_index];

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public bool MoveNext()
                {
                    int index = _index + 1, count = source.Count;
                    if (index < count)
                    {
                        _index = index;
                        return true;
                    }
                    _index = count;
                    return false;
                }

                public void Reset() => _index = -1;

                object? IEnumerator.Current => Current;

                void IDisposable.Dispose() { }
            }
        }
    }
}
