using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        /// <summary>
        /// A base class for enumerables that are loaded on-demand.
        /// </summary>
        /// <typeparam name="TSource">The type of each item to yield.</typeparam>
        /// <remarks>
        /// <list type="bullet">
        /// <item><description>
        /// The value of an iterator is immutable; the operation it represents cannot be changed.
        /// </description></item>
        /// <item><description>
        /// However, an iterator also serves as its own enumerator, so the state of an iterator
        /// may change as it is being enumerated.
        /// </description></item>
        /// <item><description>
        /// Hence, state that is relevant to an iterator's value should be kept in readonly fields.
        /// State that is relevant to an iterator's enumeration (such as the currently yielded item)
        /// should be kept in non-readonly fields.
        /// </description></item>
        /// </list>
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
