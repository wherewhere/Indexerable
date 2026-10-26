using System;
using System.Collections;
using System.Collections.Generic;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        private abstract partial class IndexerBase<T> : IList, IReadOnlyList<T>, IList<T>
        {
            public abstract T this[int index] { get; }
            public abstract int Count { get; }
            public bool IsReadOnly => true;

            public abstract bool Contains(T item);
            public virtual bool Contains(object? value) => value switch
            {
                null when default(T) is null => Contains(default!),
                T item => Contains(item),
                _ => false,
            };

            public abstract int IndexOf(T item);
            public virtual int IndexOf(object? value) => value switch
            {
                null when default(T) is null => IndexOf(default!),
                T item => IndexOf(item),
                _ => -1,
            };

            public abstract void CopyTo(T[] array, int arrayIndex);
            public virtual void CopyTo(Array array, int index)
            {
                if (array is not T[] typedArray)
                {
                    throw new ArgumentException("Target array type is not compatible with the type of items in the collection.", nameof(array));
                }
                CopyTo(typedArray, index);
            }

            public abstract IEnumerator<T> GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            #region Explicit Interface Implementations

            bool ICollection.IsSynchronized => false;
            object ICollection.SyncRoot => this;
            bool IList.IsFixedSize => true;
            T IList<T>.this[int index]
            {
                get => this[index];
                set => throw new NotSupportedException();
            }
            object? IList.this[int index]
            {
                get => this[index];
                set => throw new NotSupportedException();
            }

            #endregion

            #region Not Supported Operations

            int IList.Add(object? value) => throw new NotSupportedException();
            void IList.Clear() => throw new NotSupportedException();
            void IList.Insert(int index, object? value) => throw new NotSupportedException();
            void IList.Remove(object? value) => throw new NotSupportedException();
            void IList.RemoveAt(int index) => throw new NotSupportedException();
            void ICollection<T>.Add(T item) => throw new NotSupportedException();
            void ICollection<T>.Clear() => throw new NotSupportedException();
            bool ICollection<T>.Remove(T item) => throw new NotSupportedException();
            void IList<T>.Insert(int index, T item) => throw new NotSupportedException();
            void IList<T>.RemoveAt(int index) => throw new NotSupportedException();

            #endregion

            protected abstract partial class IndexerEnumeratorBase : IEnumerator<T>
            {
                protected int _index = -1;
                public abstract T Current { get; }
                public abstract bool MoveNext();
                public virtual void Reset() => _index = -1;
                object? IEnumerator.Current => Current;
                public virtual void Dispose() { }
            }

            protected abstract partial class EnumeratorIndexerEnumeratorBase : IndexerEnumeratorBase, IDisposable
            {
                protected IEnumerator<T>? _enumerator = null;
                protected T _current = default!;
                public sealed override T Current => _current;
                public sealed override void Reset() { base.Reset(); _current = default!; }
                public sealed override void Dispose() { _enumerator?.Dispose(); _enumerator = null; }
            }
        }

        /// <summary>
        /// A base class for read-only lists whose elements are loaded on demand.
        /// </summary>
        /// <typeparam name="T">The type of each item to yield.</typeparam>
        /// <remarks>
        /// Each instance represents a fixed list operation. The base implementation creates a separate
        /// enumerator whose position changes as the list is enumerated.
        /// </remarks>
        private abstract partial class Indexer<T> : IndexerBase<T>
        {
            public override bool Contains(T item)
            {
                EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
                foreach (T element in this)
                {
                    if (equalityComparer.Equals(element, item))
                    {
                        return true;
                    }
                }
                return false;
            }

            public override int IndexOf(T item)
            {
                int count = Count;
                EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
                for (int i = 0; i < count; i++)
                {
                    if (equalityComparer.Equals(this[i], item))
                    {
                        return i;
                    }
                }
                return -1;
            }

            public override void CopyTo(T[] array, int arrayIndex)
            {
                int count = Count;
                for (int i = 0; i < count; i++)
                {
                    array[arrayIndex + i] = this[i];
                }
            }

            public sealed override IEnumerator<T> GetEnumerator() => new IndexerEnumerator(this);

            protected sealed partial class IndexerEnumerator(Indexer<T> source) : IndexerEnumeratorBase
            {
                public override T Current => source[_index];

                public override bool MoveNext()
                {
                    int index = _index + 1, count = source.Count;
                    if ((uint)index < (uint)count)
                    {
                        _index = index;
                        return true;
                    }
                    _index = count;
                    return false;
                }
            }
        }
    }
}
