using System.Collections;
using System.Collections.Generic;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        public static IReadOnlyList<TSource> Append<TSource>(this IReadOnlyList<TSource> source, TSource element)
        {
            if (source is null) { ThrowHelper.ThrowArgumentNullException(nameof(source)); }
            return source is IAppend<TSource> append
                ? append.Append(element) : new AppendIndexer<TSource>(source, element);
        }

        private interface IAppend<TSource> : IReadOnlyList<TSource>
        {
            IReadOnlyList<TSource> Append(TSource element);
        }

        /// <summary>
        /// Represents the insertion of one or more items after an <see cref="IReadOnlyList{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the source enumerable.</typeparam>
        private sealed partial class AppendIndexer<TSource>(IReadOnlyList<TSource> source, TSource element) : IAppendPrepend<TSource>
        {
            public TSource this[int index] => index == source.Count ? element : source[index];

            public int Count => checked(source.Count + 1);

            public IReadOnlyList<TSource> Append(TSource _element) => new AppendNIndexer<TSource>(source, element, _element);

            public IReadOnlyList<TSource> Prepend(TSource _element) => new AppendPrependIndexer<TSource>(source, _element, element);

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
        /// Represents the insertion of multiple items after an <see cref="IEnumerable{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the source enumerable.</typeparam>
        private sealed partial class AppendNIndexer<TSource>(IReadOnlyList<TSource> source, params TSource[] appended) : IAppendPrepend<TSource>
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

        public static IReadOnlyList<TSource> Prepend<TSource>(this IReadOnlyList<TSource> source, TSource element)
        {
            if (source is null) { ThrowHelper.ThrowArgumentNullException(nameof(source)); }
            return source is IPrepend<TSource> prepend
                ? prepend.Prepend(element) : new PrependIndexer<TSource>(source, element);
        }

        private interface IPrepend<TSource> : IReadOnlyList<TSource>
        {
            IReadOnlyList<TSource> Prepend(TSource element);
        }

        /// <summary>
        /// Represents the insertion of one or more items before an <see cref="IReadOnlyList{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the source enumerable.</typeparam>
        private sealed partial class PrependIndexer<TSource>(IReadOnlyList<TSource> source, TSource element) : IAppendPrepend<TSource>
        {
            public TSource this[int index] => index == 0 ? element : source[index - 1];

            public int Count => checked(source.Count + 1);

            public IReadOnlyList<TSource> Append(TSource _element) => new AppendPrependIndexer<TSource>(source, element, _element);

            public IReadOnlyList<TSource> Prepend(TSource _element) => new PrependNIndexer<TSource>(source, _element, element);

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
        /// Represents the insertion of multiple items before an <see cref="IEnumerable{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the source enumerable.</typeparam>
        private sealed partial class PrependNIndexer<TSource>(IReadOnlyList<TSource> source, params TSource[] prepended) : IAppendPrepend<TSource>
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
        /// Represents the insertion of one or more items before or after an <see cref="IEnumerable{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the source enumerable.</typeparam>
        private sealed partial class AppendPrependIndexer<TSource>(IReadOnlyList<TSource> source, TSource prepended, TSource appended) : IAppendPrepend<TSource>
        {
            public TSource this[int index] => index == 0 ? prepended : index == source.Count + 1 ? appended : source[index - 1];

            public int Count => checked(1 + source.Count + 1);

            public IReadOnlyList<TSource> Append(TSource _element) => new AppendPrependNIndexer<TSource>(source, [prepended], [appended, _element]);

            public IReadOnlyList<TSource> Prepend(TSource _element) => new AppendPrependNIndexer<TSource>(source, [_element, prepended], [appended]);

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
        /// Represents the insertion of multiple items before or after an <see cref="IEnumerable{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the source enumerable.</typeparam>
        private sealed partial class AppendPrependNIndexer<TSource>(IReadOnlyList<TSource> source, TSource[] prepended, TSource[] appended) : IAppendPrepend<TSource>
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
