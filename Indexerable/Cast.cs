using System;
using System.Collections;
using System.Collections.Generic;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        public static IReadOnlyList<TResult> Cast<TResult>(this IList source)
        {
            if (source is IReadOnlyList<TResult> typedSource)
            { return typedSource; }
            ArgumentNullException.ThrowIfNull(source);
            return new CastIndexer<TResult>(source);
        }

        private sealed partial class CastIndexer<TResult>(IList source) : Indexer<TResult>
        {
            public override TResult this[int index] => (TResult)source[index]!;
            public override int Count => source.Count;
        }
    }
}
