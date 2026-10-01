using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
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

        public static IReadOnlyList<TResult> Cast<TSource, TResult>(this IReadOnlyList<TSource> source)
        {
            if (source is IReadOnlyList<TResult> typedSource) { return typedSource; }
            else if (source is null) { ThrowHelper.ThrowArgumentNullException(nameof(source)); }
            return source is Array { Rank: > 1 } array ? new ArrayCastIndexer<TResult>(array) : new CastIndexer<TSource, TResult>(source);
        }

        private sealed partial class CastIndexer<TSource, TResult>(IReadOnlyList<TSource> source) : Indexer<TResult>
        {
            public override TResult this[int index] => (TResult)(object)source[index]!;
            public override int Count => source.Count;
        }

        private sealed partial class ArrayCastIndexer<TResult>(Array source) : IReadOnlyList<TResult>
        {
            public TResult this[int index] => (TResult)source.GetValue(index)!;
            public int Count => source.Length;
            public IEnumerator<TResult> GetEnumerator() => Enumerable.Cast<TResult>(source).GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
