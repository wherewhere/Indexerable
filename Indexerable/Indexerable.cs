using System;
using System.Collections;
using System.Collections.Generic;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        public static IReadOnlyList<TSource> AsReadOnlyList<TSource>(this IReadOnlyList<TSource> source) => source;

        public static IReadOnlyList<TSource> ToReadOnlyList<TSource>(this IList<TSource> source)
        {
            ArgumentNullException.ThrowIfNull(source);
            return source is IReadOnlyList<TSource> typedSource ? typedSource : new ReadOnlyIndexer<TSource>(source);
        }

        /// <summary>Returns an empty <see cref="IReadOnlyList{TResult}"/>.</summary>
        public static IReadOnlyList<TResult> Empty<TResult>() => [];

        private sealed partial class ReadOnlyIndexer<TSource>(IList<TSource> source) : IReadOnlyList<TSource>
        {
            public TSource this[int index] => source[index];
            public int Count => source.Count;
            public IEnumerator<TSource> GetEnumerator() => source.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
