using System;
using System.Collections.Generic;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        private abstract partial class SkipIndexerBase<TSource>(IReadOnlyList<TSource> source) : IndexerBase<TSource>
        {
            protected readonly IReadOnlyList<TSource> source = source;

            protected abstract int GetCount(out int offset);

            public sealed override TSource this[int index] =>
                index >= 0 && (uint)index < (uint)GetCount(out int offset)
                    ? source[offset + index]
                    : throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

            public sealed override bool Contains(TSource item)
            {
                int count = GetCount(out int offset);
                for (int i = 0; i < count; i++)
                {
                    TSource j = source[offset + i];
                    if (EqualityComparer<TSource>.Default.Equals(j, item))
                    {
                        return true;
                    }
                }
                return false;
            }

            public sealed override int IndexOf(TSource item)
            {
                int count = GetCount(out int offset);
                for (int i = 0; i < count; i++)
                {
                    TSource j = source[offset + i];
                    if (EqualityComparer<TSource>.Default.Equals(j, item))
                    {
                        return i;
                    }
                }
                return -1;
            }

            public sealed override void CopyTo(TSource[] array, int arrayIndex)
            {
                int count = GetCount(out int offset);
                for (int i = 0; i < count; i++)
                {
                    array[arrayIndex + i] = source[offset + i];
                }
            }

            public sealed override IEnumerator<TSource> GetEnumerator()
            {
                int count = GetCount(out int offset);
                return new IndexerEnumerator(source, count, offset);
            }

            private sealed partial class IndexerEnumerator(IReadOnlyList<TSource> source, int count, int offset) : IndexerEnumeratorBase
            {
                public override TSource Current => source[offset + _index];

                public override bool MoveNext()
                {
                    int index = _index + 1;
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

        private abstract partial class TakeIndexerBase<TSource>(IReadOnlyList<TSource> source) : IndexerBase<TSource>
        {
            protected readonly IReadOnlyList<TSource> source = source;

            public sealed override TSource this[int index] =>
                index >= 0 && (uint)index < (uint)Count
                    ? source[index]
                    : throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

            public sealed override bool Contains(TSource item)
            {
                int count = Count;
                for (int i = 0; i < count; i++)
                {
                    TSource j = source[i];
                    if (EqualityComparer<TSource>.Default.Equals(j, item))
                    {
                        return true;
                    }
                }
                return false;
            }

            public sealed override int IndexOf(TSource item)
            {
                int count = Count;
                for (int i = 0; i < count; i++)
                {
                    TSource j = source[i];
                    if (EqualityComparer<TSource>.Default.Equals(j, item))
                    {
                        return i;
                    }
                }
                return -1;
            }

            public sealed override void CopyTo(TSource[] array, int arrayIndex)
            {
                int count = Count;
                for (int i = 0; i < count; i++)
                {
                    array[arrayIndex + i] = source[i];
                }
            }

            public sealed override IEnumerator<TSource> GetEnumerator() => new IndexerEnumerator(source, Count);

            private sealed partial class IndexerEnumerator(IReadOnlyList<TSource> source, int count) : IndexerEnumeratorBase
            {
                public override TSource Current => source[_index];

                public override bool MoveNext()
                {
                    int index = _index + 1;
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
