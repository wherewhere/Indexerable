using System;
using System.Collections;
using System.Collections.Generic;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        private abstract partial class IndexerBase<TSource> : IList, IReadOnlyList<TSource>, IList<TSource>
        {
            public abstract TSource this[int index] { get; }
            public abstract int Count { get; }
            public bool IsReadOnly => true;

            public abstract bool Contains(TSource item);
            public virtual bool Contains(object? value) => value switch
            {
                null when default(TSource) is null => Contains(default!),
                TSource item => Contains(item),
                _ => false,
            };

            public abstract int IndexOf(TSource item);
            public virtual int IndexOf(object? value) => value switch
            {
                null when default(TSource) is null => IndexOf(default!),
                TSource item => IndexOf(item),
                _ => -1,
            };

            public abstract void CopyTo(TSource[] array, int arrayIndex);
            public virtual void CopyTo(Array array, int index)
            {
                if (array is not TSource[] typedArray)
                {
                    throw new ArgumentException("Target array type is not compatible with the type of items in the collection.", nameof(array));
                }
                CopyTo(typedArray, index);
            }

            public abstract IEnumerator<TSource> GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            #region Explicit Interface Implementations

            bool ICollection.IsSynchronized => false;
            object ICollection.SyncRoot => this;
            bool IList.IsFixedSize => true;
            TSource IList<TSource>.this[int index]
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
            void ICollection<TSource>.Add(TSource item) => throw new NotSupportedException();
            void ICollection<TSource>.Clear() => throw new NotSupportedException();
            bool ICollection<TSource>.Remove(TSource item) => throw new NotSupportedException();
            void IList<TSource>.Insert(int index, TSource item) => throw new NotSupportedException();
            void IList<TSource>.RemoveAt(int index) => throw new NotSupportedException();

            #endregion

            protected abstract partial class IndexerEnumeratorBase : IEnumerator<TSource>
            {
                protected int _index = -1;
                public abstract TSource Current { get; }
                public abstract bool MoveNext();
                public virtual void Reset() => _index = -1;
                object? IEnumerator.Current => Current;
                public virtual void Dispose() { }
            }

            protected abstract partial class EnumeratorIndexerEnumeratorBase : IndexerEnumeratorBase, IDisposable
            {
                protected IEnumerator<TSource>? _enumerator = null;
                protected TSource _current = default!;
                public sealed override TSource Current => _current;
                public sealed override void Reset() { base.Reset(); _current = default!; }
                public sealed override void Dispose() { _enumerator?.Dispose(); _enumerator = null; }
            }
        }

        /// <summary>
        /// A base class for read-only lists whose elements are loaded on demand.
        /// </summary>
        /// <typeparam name="TSource">The type of each item to yield.</typeparam>
        /// <remarks>
        /// Each instance represents a fixed list operation. The base implementation creates a separate
        /// enumerator whose position changes as the list is enumerated.
        /// </remarks>
        private abstract partial class Indexer<TSource> : IndexerBase<TSource>
        {
            public override bool Contains(TSource item)
            {
                foreach (TSource element in this)
                {
                    if (EqualityComparer<TSource>.Default.Equals(element, item))
                    {
                        return true;
                    }
                }
                return false;
            }

            public override int IndexOf(TSource item)
            {
                int count = Count;
                for (int i = 0; i < count; i++)
                {
                    if (EqualityComparer<TSource>.Default.Equals(this[i], item))
                    {
                        return i;
                    }
                }
                return -1;
            }

            public override void CopyTo(TSource[] array, int arrayIndex)
            {
                int count = Count;
                for (int i = 0; i < count; i++)
                {
                    array[arrayIndex + i] = this[i];
                }
            }

            public sealed override IEnumerator<TSource> GetEnumerator() => new IndexerEnumerator(this);

            protected sealed partial class IndexerEnumerator(Indexer<TSource> source) : IndexerEnumeratorBase
            {
                public override TSource Current => source[_index];

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
