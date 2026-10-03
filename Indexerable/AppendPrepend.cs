using System.Collections;
using System.Collections.Generic;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        /// <summary>
        /// Adds a value to the end of a read-only list.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The list to append a value to.</param>
        /// <param name="element">The value to append to <paramref name="source"/>.</param>
        /// <returns>A read-only list that contains the elements of <paramref name="source"/> followed by <paramref name="element"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        public static IReadOnlyList<TSource> Append<TSource>(this IReadOnlyList<TSource> source, TSource element)
        {
            if (source is null) { ThrowHelper.ThrowArgumentNullException(nameof(source)); }
            return source is IAppend<TSource> append
                ? append.Append(element) : new AppendIndexer<TSource>(source, element);
        }

        private interface IAppend<TSource> : IReadOnlyList<TSource>
        {
            /// <inheritdoc cref="Append{TSource}(IReadOnlyList{TSource}, TSource)"/>
            IReadOnlyList<TSource> Append(TSource element);
        }

        /// <summary>
        /// Represents the insertion of one or more items after an <see cref="IReadOnlyList{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the source list.</typeparam>
        private sealed partial class AppendIndexer<TSource>(IReadOnlyList<TSource> source, TSource element) : IAppendPrepend<TSource>, ISkipLast<TSource>, ITakeLast<TSource>
        {
            public TSource this[int index] => index == source.Count ? element : source[index];

            public int Count => checked(source.Count + 1);

            public IReadOnlyList<TSource> Append(TSource _element) => new AppendNIndexer<TSource>(source, element, _element);

            public IReadOnlyList<TSource> Prepend(TSource _element) => new AppendPrependIndexer<TSource>(source, _element, element);

            public IReadOnlyList<TSource> SkipLast(int count) =>
                count <= 0 ? this
                    : count == 1 ? source
                    : source is ISkipLast<TSource> skip ? skip.SkipLast(count - 1)
                    : new SkipLastIndexer<TSource>(source, count - 1);

            public IReadOnlyList<TSource> TakeLast(int count) =>
                count <= 0 ? (TSource[])[]
                    : count == 1 ? (TSource[])[element]
                    : new TakeLastIndexer<TSource>(this, count);

            public IEnumerator<TSource> GetEnumerator()
            {
                foreach (TSource? item in source)
                {
                    yield return item;
                }
                yield return element;
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        /// <summary>
        /// Represents the insertion of multiple items after an <see cref="IReadOnlyList{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the source list.</typeparam>
        private sealed partial class AppendNIndexer<TSource>(IReadOnlyList<TSource> source, params TSource[] appended) : IAppendPrepend<TSource>, ISkipLast<TSource>, ITakeLast<TSource>
        {
            public TSource this[int index]
            {
                get
                {
                    int sourceCount;
                    return index < (sourceCount = source.Count) ? source[index] : appended[index - sourceCount];
                }
            }

            public int Count => checked(source.Count + appended.Length);

            public IReadOnlyList<TSource> Append(TSource _element) => new AppendNIndexer<TSource>(source, [.. appended, _element]);

            public IReadOnlyList<TSource> Prepend(TSource _element) => new AppendPrependNIndexer<TSource>(source, [_element], appended);

            public IReadOnlyList<TSource> SkipLast(int count)
            {
                int length;
                return count <= 0 ? this
                    : count < (length = appended.Length) ? new AppendNIndexer<TSource>(source, appended[..^count])
                    : count == length ? source
                    : source is ISkipLast<TSource> skip ? skip.SkipLast(count - length)
                    : new SkipLastIndexer<TSource>(source, count - length);
            }

            public IReadOnlyList<TSource> TakeLast(int count)
            {
                int length;
                return count <= 0 ? (TSource[])[]
                    : count < (length = appended.Length) ? appended[^count..]
                    : count == length ? appended
                    : new TakeLastIndexer<TSource>(this, count);
            }

            public IEnumerator<TSource> GetEnumerator()
            {
                foreach (TSource? item in source)
                {
                    yield return item;
                }
                foreach (TSource? item in appended)
                {
                    yield return item;
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        /// <summary>
        /// Adds a value to the beginning of a read-only list.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The list to prepend a value to.</param>
        /// <param name="element">The value to prepend to <paramref name="source"/>.</param>
        /// <returns>A read-only list that contains <paramref name="element"/> followed by the elements of <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        public static IReadOnlyList<TSource> Prepend<TSource>(this IReadOnlyList<TSource> source, TSource element)
        {
            if (source is null) { ThrowHelper.ThrowArgumentNullException(nameof(source)); }
            return source is IPrepend<TSource> prepend
                ? prepend.Prepend(element) : new PrependIndexer<TSource>(source, element);
        }

        private interface IPrepend<TSource> : IReadOnlyList<TSource>
        {
            /// <inheritdoc cref="Prepend{TSource}(IReadOnlyList{TSource}, TSource)"/>
            IReadOnlyList<TSource> Prepend(TSource element);
        }

        /// <summary>
        /// Represents the insertion of one or more items before an <see cref="IReadOnlyList{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the source list.</typeparam>
        private sealed partial class PrependIndexer<TSource>(IReadOnlyList<TSource> source, TSource element) : IAppendPrepend<TSource>, ISkip<TSource>, ITake<TSource>
        {
            public TSource this[int index] => index == 0 ? element : source[index - 1];

            public int Count => checked(source.Count + 1);

            public IReadOnlyList<TSource> Append(TSource _element) => new AppendPrependIndexer<TSource>(source, element, _element);

            public IReadOnlyList<TSource> Prepend(TSource _element) => new PrependNIndexer<TSource>(source, _element, element);

            public IReadOnlyList<TSource> Skip(int count) =>
                count <= 0 ? this
                    : count == 1 ? source
                    : source is ISkip<TSource> skip ? skip.Skip(count - 1)
                    : new SkipIndexer<TSource>(source, count - 1);

            public IReadOnlyList<TSource> Take(int count) =>
                count <= 0 ? (TSource[])[]
                    : count == 1 ? (TSource[])[element]
                    : new TakeIndexer<TSource>(this, count);

            public IEnumerator<TSource> GetEnumerator()
            {
                yield return element;
                foreach (TSource? item in source)
                {
                    yield return item;
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        /// <summary>
        /// Represents the insertion of multiple items before an <see cref="IReadOnlyList{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the source list.</typeparam>
        private sealed partial class PrependNIndexer<TSource>(IReadOnlyList<TSource> source, params TSource[] prepended) : IAppendPrepend<TSource>, ISkip<TSource>, ITake<TSource>
        {
            public TSource this[int index]
            {
                get
                {
                    int prependedCount;
                    return index < (prependedCount = prepended.Length) ? prepended[index] : source[index - prependedCount];
                }
            }

            public int Count => checked(prepended.Length + source.Count);

            public IReadOnlyList<TSource> Append(TSource _element) => new AppendPrependNIndexer<TSource>(source, prepended, [_element]);

            public IReadOnlyList<TSource> Prepend(TSource _element) => new PrependNIndexer<TSource>(source, [_element, .. prepended]);

            public IReadOnlyList<TSource> Skip(int count)
            {
                int length;
                return count <= 0 ? this
                    : count < (length = prepended.Length) ? new PrependNIndexer<TSource>(source, prepended[count..])
                    : count == length ? source
                    : source is ISkip<TSource> skip ? skip.Skip(count - length)
                    : new SkipIndexer<TSource>(source, count - length);
            }

            public IReadOnlyList<TSource> Take(int count)
            {
                int length;
                return count <= 0 ? (TSource[])[]
                    : count < (length = prepended.Length) ? prepended[..count]
                    : count == length ? prepended
                    : new TakeIndexer<TSource>(this, count);
            }

            public IEnumerator<TSource> GetEnumerator()
            {
                foreach (TSource? item in prepended)
                {
                    yield return item;
                }
                foreach (TSource? item in source)
                {
                    yield return item;
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private interface IAppendPrepend<TSource> : IAppend<TSource>, IPrepend<TSource>;

        /// <summary>
        /// Represents the insertion of one or more items before or after an <see cref="IReadOnlyList{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the source list.</typeparam>
        private sealed partial class AppendPrependIndexer<TSource>(IReadOnlyList<TSource> source, TSource prepended, TSource appended) : IAppendPrepend<TSource>, ISkip<TSource>, ISkipLast<TSource>, ITake<TSource>, ITakeLast<TSource>
        {
            public TSource this[int index] => index == 0 ? prepended : index == source.Count + 1 ? appended : source[index - 1];

            public int Count => checked(1 + source.Count + 1);

            public IReadOnlyList<TSource> Append(TSource _element) => new AppendPrependNIndexer<TSource>(source, [prepended], [appended, _element]);

            public IReadOnlyList<TSource> Prepend(TSource _element) => new AppendPrependNIndexer<TSource>(source, [_element, prepended], [appended]);

            public IReadOnlyList<TSource> Skip(int count)
            {
                if (count > 0)
                {
                    AppendIndexer<TSource> indexer = new(source, appended);
                    return count == 1 ? indexer : new SkipIndexer<TSource>(indexer, count - 1);
                }
                return this;
            }

            public IReadOnlyList<TSource> SkipLast(int count)
            {
                if (count > 0)
                {
                    PrependIndexer<TSource> indexer = new(source, prepended);
                    return count == 1 ? indexer : new SkipLastIndexer<TSource>(indexer, count - 1);
                }
                return this;
            }

            public IReadOnlyList<TSource> Take(int count) =>
                count <= 0 ? (TSource[])[]
                    : count == 1 ? (TSource[])[prepended]
                    : new TakeIndexer<TSource>(this, count);

            public IReadOnlyList<TSource> TakeLast(int count) =>
                count <= 0 ? (TSource[])[]
                    : count == 1 ? (TSource[])[appended]
                    : new TakeLastIndexer<TSource>(this, count);

            public IEnumerator<TSource> GetEnumerator()
            {
                yield return prepended;
                foreach (TSource? item in source)
                {
                    yield return item;
                }
                yield return appended;
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        /// <summary>
        /// Represents the insertion of multiple items before or after an <see cref="IReadOnlyList{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the source list.</typeparam>
        private sealed partial class AppendPrependNIndexer<TSource>(IReadOnlyList<TSource> source, TSource[] prepended, TSource[] appended) : IAppendPrepend<TSource>, ISkip<TSource>, ISkipLast<TSource>, ITake<TSource>, ITakeLast<TSource>
        {
            public TSource this[int index]
            {
                get
                {
                    int prependedCount, prependedSourceCount;
                    return index < (prependedCount = prepended.Length) ? prepended[index]
                        : index < (prependedSourceCount = prependedCount + source.Count) ? source[index - prependedCount]
                        : appended[index - prependedSourceCount];
                }
            }

            public int Count => checked(prepended.Length + source.Count + appended.Length);

            public IReadOnlyList<TSource> Append(TSource _element) => new AppendPrependNIndexer<TSource>(source, prepended, [.. appended, _element]);

            public IReadOnlyList<TSource> Prepend(TSource _element) => new AppendPrependNIndexer<TSource>(source, [_element, .. prepended], appended);

            public IReadOnlyList<TSource> Skip(int count)
            {
                if (count > 0)
                {
                    int length;
                    if (count >= (length = prepended.Length))
                    {
                        AppendNIndexer<TSource> indexer = new(source, appended);
                        return count == length ? indexer
                            : new SkipIndexer<TSource>(indexer, count - length);
                    }
                    return new AppendPrependNIndexer<TSource>(source, prepended[count..], appended);
                }
                return this;
            }

            public IReadOnlyList<TSource> SkipLast(int count)
            {
                if (count > 0)
                {
                    int length;
                    if (count >= (length = appended.Length))
                    {
                        PrependNIndexer<TSource> indexer = new(source, prepended);
                        return count == length ? indexer
                            : new SkipLastIndexer<TSource>(indexer, count - length);
                    }
                    return new AppendPrependNIndexer<TSource>(source, prepended, appended[..^count]);
                }
                return this;
            }

            public IReadOnlyList<TSource> Take(int count)
            {
                int length;
                return count <= 0 ? (TSource[])[]
                    : count < (length = prepended.Length) ? prepended[..count]
                    : count == length ? prepended
                    : new TakeIndexer<TSource>(this, count);
            }

            public IReadOnlyList<TSource> TakeLast(int count)
            {
                int length;
                return count <= 0 ? (TSource[])[]
                    : count < (length = appended.Length) ? appended[^count..]
                    : count == length ? appended
                    : new TakeLastIndexer<TSource>(this, count);
            }

            public IEnumerator<TSource> GetEnumerator()
            {
                foreach (TSource? item in prepended)
                {
                    yield return item;
                }
                foreach (TSource? item in source)
                {
                    yield return item;
                }
                foreach (TSource? item in appended)
                {
                    yield return item;
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
