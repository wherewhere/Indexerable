using System;
using System.Collections;
using System.Collections.Generic;

namespace Indexer.Linq
{
    /// <summary>
    /// Provides static methods for creating and querying read-only lists.
    /// </summary>
    public static partial class Indexerable
    {
        /// <summary>
        /// Returns the specified read-only list as a read-only list without copying it.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the list.</typeparam>
        /// <param name="source">The read-only list to return.</param>
        /// <returns>The same read-only list instance.</returns>
        public static IReadOnlyList<TSource> AsReadOnlyList<TSource>(this IReadOnlyList<TSource> source) => source;

        /// <summary>
        /// Provides read-only list access to the elements of a list.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the list.</typeparam>
        /// <param name="source">The list whose elements are exposed as a read-only list.</param>
        /// <returns>A read-only list that provides access to the elements of <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <remarks>The returned list wraps <paramref name="source"/>; it does not copy its elements.</remarks>
        public static IReadOnlyList<TSource> ToReadOnlyList<TSource>(this IList<TSource> source)
        {
            if (source is null) { ThrowHelper.ThrowArgumentNullException(nameof(source)); }
            return source is IReadOnlyList<TSource> typedSource ? typedSource : new ReadOnlyListIndexer<TSource>(source);
        }

        private sealed partial class ReadOnlyListIndexer<TSource>(IList<TSource> source) : IndexerBase<TSource>
        {
            public override TSource this[int index] => source[index];
            public override int Count => source.Count;
            public override bool Contains(TSource item) => source.Contains(item);
            public override bool Contains(object? value) => (source is IList list && list.Contains(value)) || base.Contains(value);
            public override int IndexOf(TSource item) => source.IndexOf(item);
            public override int IndexOf(object? value) => source is IList list ? list.IndexOf(value) : base.IndexOf(value);
            public override void CopyTo(TSource[] array, int arrayIndex) => source.CopyTo(array, arrayIndex);
            public override void CopyTo(Array array, int index) { if (source is ICollection collection) { collection.CopyTo(array, index); } else { base.CopyTo(array, index); } }
            public override IEnumerator<TSource> GetEnumerator() => source.GetEnumerator();
        }

        /// <summary>
        /// Provides read-only list access to the characters in a string.
        /// </summary>
        /// <param name="source">The string whose characters are exposed as a read-only list.</param>
        /// <returns>A read-only list containing the characters in <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        public static IReadOnlyList<char> ToReadOnlyList(this string source)
        {
            if (source is null) { ThrowHelper.ThrowArgumentNullException(nameof(source)); }
            return new ReadOnlyStringIndexer(source);
        }

        private sealed partial class ReadOnlyStringIndexer(string source) : IndexerBase<char>
        {
            public override char this[int index] => source[index];
            public override int Count => source.Length;
            public override bool Contains(char item) => source.Contains(item);
            public override int IndexOf(char item) => source.IndexOf(item);
            public override void CopyTo(char[] array, int arrayIndex) => source.CopyTo(0, array, arrayIndex, source.Length);
#if !NETSTANDARD || NETSTANDARD1_2_OR_GREATER
            public override IEnumerator<char> GetEnumerator() => ((IEnumerable<char>)source).GetEnumerator();
#else
            public override IEnumerator<char> GetEnumerator() => ((IEnumerable<char>)source.ToCharArray()).GetEnumerator();
#endif
            public override string ToString() => source;
        }

        /// <summary>
        /// Returns an empty read-only list of the specified type.
        /// </summary>
        /// <typeparam name="TResult">The type of the elements of the list.</typeparam>
        /// <returns>An empty <see cref="IReadOnlyList{TResult}"/>.</returns>
        public static IReadOnlyList<TResult> Empty<TResult>() => EmptyIndexer<TResult>.Instance;

        private sealed partial class EmptyIndexer<T>() : IndexerBase<T>, ISelectWithIndex<T>, IAppendPrepend<T>, ISkipBoth<T>, ITake<T>, ITakeLast<T>, IConcat<T>
        {
            public static EmptyIndexer<T> Instance { get; } = new();
            public override T this[int index] => throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
            public override int Count => 0;
            public IReadOnlyList<TResult> Select<TResult>(Func<T, TResult> _selector) => new EmptyIndexer<TResult>();
            public IReadOnlyList<TResult> Select<TResult>(Func<T, int, TResult> _selector) => new EmptyIndexer<TResult>();
            public IReadOnlyList<T> Append(T element) => new SingleElementIndexer<T>(element);
            public IReadOnlyList<T> Prepend(T element) => new SingleElementIndexer<T>(element);
            public IReadOnlyList<T> Skip(int count) => this;
            public IReadOnlyList<T> SkipLast(int count) => this;
            public IReadOnlyList<T> Take(int count) => this;
            public IReadOnlyList<T> TakeLast(int count) => this;
            public IReadOnlyList<T> Concat(IReadOnlyList<T> second) => second;
            public override bool Contains(T item) => false;
            public override int IndexOf(T item) => -1;
            public override void CopyTo(T[] array, int arrayIndex)
            {
                if (array is null) { ThrowHelper.ThrowArgumentNullException(nameof(array)); }
                if (arrayIndex < 0) { ThrowHelper.ThrowArgumentOutOfRangeException(nameof(arrayIndex)); }
                if (arrayIndex > array.Length) { ThrowHelper.ThrowArgumentException_LongerThanDestArray(nameof(array)); }
            }
            public sealed override IEnumerator<T> GetEnumerator() => new IndexerEnumerator();
            private readonly partial struct IndexerEnumerator : IEnumerator<T>
            {
                public T Current => throw new InvalidOperationException();
                public bool MoveNext() => false;
                public void Reset() { }
                object? IEnumerator.Current => Current;
                void IDisposable.Dispose() { }
            }
        }

#if COMP_NETSTANDARD2_1
        private sealed partial class EmptyIndexer<T> : ITakeRange<T>
        {
            public IReadOnlyList<T> Take(Range range) => this;
        }
#endif
    }
}
