using System.Collections;
using System.Collections.Generic;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        public static IReadOnlyList<TSource> AsReadOnlyList<TSource>(this IReadOnlyList<TSource> source) => source;

        public static IReadOnlyList<TSource> ToReadOnlyList<TSource>(this IList<TSource> source)
        {
            if (source is null) { ThrowHelper.ThrowArgumentNullException(nameof(source)); }
            return source is IReadOnlyList<TSource> typedSource ? typedSource : new ReadOnlyListIndexer<TSource>(source);
        }

        public static IReadOnlyList<char> ToReadOnlyList(this string source)
        {
            if (source is null) { ThrowHelper.ThrowArgumentNullException(nameof(source)); }
            return new ReadOnlyStringIndexer(source);
        }

        /// <summary>Returns an empty <see cref="IReadOnlyList{TResult}"/>.</summary>
        public static IReadOnlyList<TResult> Empty<TResult>() => (TResult[])[];

        private sealed partial class ReadOnlyListIndexer<TSource>(IList<TSource> source) : IReadOnlyList<TSource>
        {
            public TSource this[int index] => source[index];
            public int Count => source.Count;
            public IEnumerator<TSource> GetEnumerator() => source.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private sealed partial class ReadOnlyStringIndexer(string source) : IReadOnlyList<char>
        {
            public char this[int index] => source[index];
            public int Count => source.Length;
#if !NETSTANDARD || NETSTANDARD1_2_OR_GREATER
            public IEnumerator<char> GetEnumerator() => ((IEnumerable<char>)source).GetEnumerator();
#else
            public IEnumerator<char> GetEnumerator() => ((IEnumerable<char>)source.ToCharArray()).GetEnumerator();
#endif
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            public override string ToString() => source;
        }
    }
}
