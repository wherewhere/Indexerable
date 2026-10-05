using System;
using System.Collections.Generic;
using System.Linq;

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
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
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
        private sealed partial class AppendIndexer<TSource>(IReadOnlyList<TSource> source, TSource element) : IndexerBase<TSource>, IAppendPrepend<TSource>, ISkipLast<TSource>, ITakeLast<TSource>, IConcat<TSource>
        {
            public override TSource this[int index] => index == source.Count ? element : source[index];

            public override int Count => checked(source.Count + 1);

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

            public IReadOnlyList<TSource> Concat(IReadOnlyList<TSource> second) => new ConcatNIndexer<TSource>(source, [element], second);

            public override bool Contains(TSource item) => source.Contains(item) || EqualityComparer<TSource>.Default.Equals(element, item);

            public override int IndexOf(TSource item)
            {
                int result = source.IndexOf(item);
                return result >= 0 ? result : EqualityComparer<TSource>.Default.Equals(element, item) ? source.Count : -1;
            }

            public override void CopyTo(TSource[] array, int arrayIndex)
            {
                source.CopyTo(array, arrayIndex);
                array[arrayIndex + source.Count] = element;
            }

            public override IEnumerator<TSource> GetEnumerator() => new IndexerEnumerator(source, element);

            private sealed partial class IndexerEnumerator(IReadOnlyList<TSource> source, TSource appended) : EnumeratorIndexerEnumeratorBase
            {
                public override bool MoveNext()
                {
                    switch (_index)
                    {
                        case -1:
                            _enumerator = source.GetEnumerator();
                            _index = 0;
                            goto case 0;
                        case 0:
                            if (!_enumerator!.MoveNext())
                            {
                                _index = 1;
                                _enumerator.Dispose();
                                _enumerator = null;
                                goto case 1;
                            }
                            _current = _enumerator.Current;
                            return true;
                        case 1:
                            _current = appended;
                            _index = 2;
                            return true;
                        case 2:
                        default:
                            return false;
                    }
                }
            }
        }

        /// <summary>
        /// Represents the insertion of multiple items after an <see cref="IReadOnlyList{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the source list.</typeparam>
        private sealed partial class AppendNIndexer<TSource>(IReadOnlyList<TSource> source, params TSource[] appended) : ConcatIndexerBase<TSource>, IAppendPrepend<TSource>, ISkipLast<TSource>, ITakeLast<TSource>, IConcat<TSource>
        {
            public override TSource this[int index]
            {
                get
                {
                    int sourceCount;
                    return index < (sourceCount = source.Count) ? source[index] : appended[index - sourceCount];
                }
            }

            public override int Count => checked(source.Count + appended.Length);

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

            public IReadOnlyList<TSource> Concat(IReadOnlyList<TSource> second) => new ConcatNIndexer<TSource>(source, appended, second);

            public override bool Contains(TSource item) => source.Contains(item) || ((ICollection<TSource>)appended).Contains(item);

            public override int IndexOf(TSource item)
            {
                int result = source.IndexOf(item);
                if (result >= 0)
                {
                    return result;
                }
                result = Array.IndexOf(appended, item);
                return result >= 0 ? source.Count + result : -1;
            }

            public override void CopyTo(TSource[] array, int arrayIndex)
            {
                source.CopyTo(array, arrayIndex);
                appended.CopyTo(array, arrayIndex + source.Count);
            }

            public override IEnumerator<TSource> GetEnumerator() => new IndexerEnumerator(source, appended);
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
        private sealed partial class PrependIndexer<TSource>(IReadOnlyList<TSource> source, TSource element) : IndexerBase<TSource>, IAppendPrepend<TSource>, ISkip<TSource>, ITake<TSource>, IConcat<TSource>
        {
            public override TSource this[int index] => index == 0 ? element : source[index - 1];

            public override int Count => checked(source.Count + 1);

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

            public IReadOnlyList<TSource> Concat(IReadOnlyList<TSource> second) => new ConcatNIndexer<TSource>([element], source, second);

            public override bool Contains(TSource item) => EqualityComparer<TSource>.Default.Equals(element, item) || source.Contains(item);

            public override int IndexOf(TSource item)
            {
                if (!EqualityComparer<TSource>.Default.Equals(element, item))
                {
                    int result = source.IndexOf(item);
                    return result >= 0 ? result + 1 : -1;
                }
                return 0;
            }

            public override void CopyTo(TSource[] array, int arrayIndex)
            {
                array[arrayIndex] = element;
                source.CopyTo(array, arrayIndex + 1);
            }

            public override IEnumerator<TSource> GetEnumerator() => new IndexerEnumerator(source, element);

            private sealed partial class IndexerEnumerator(IReadOnlyList<TSource> source, TSource prepended) : EnumeratorIndexerEnumeratorBase
            {
                public override bool MoveNext()
                {
                    switch (_index)
                    {
                        case -1:
                            _current = prepended;
                            _index = 0;
                            return true;
                        case 0:
                            _enumerator = source.GetEnumerator();
                            _index = 1;
                            goto case 1;
                        case 1:
                            if (!_enumerator!.MoveNext())
                            {
                                _index = 2;
                                _enumerator.Dispose();
                                _enumerator = null;
                                goto case 2;
                            }
                            _current = _enumerator.Current;
                            return true;
                        case 2:
                        default:
                            return false;
                    }
                }
            }
        }

        /// <summary>
        /// Represents the insertion of multiple items before an <see cref="IReadOnlyList{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the source list.</typeparam>
        private sealed partial class PrependNIndexer<TSource>(IReadOnlyList<TSource> source, params TSource[] prepended) : ConcatIndexerBase<TSource>, IAppendPrepend<TSource>, ISkip<TSource>, ITake<TSource>, IConcat<TSource>
        {
            public override TSource this[int index]
            {
                get
                {
                    int prependedCount;
                    return index < (prependedCount = prepended.Length) ? prepended[index] : source[index - prependedCount];
                }
            }

            public override int Count => checked(prepended.Length + source.Count);

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

            public IReadOnlyList<TSource> Concat(IReadOnlyList<TSource> second) => new ConcatNIndexer<TSource>(prepended, source, second);

            public override bool Contains(TSource item) => ((ICollection<TSource>)prepended).Contains(item) || source.Contains(item);

            public override int IndexOf(TSource item)
            {
                int result = Array.IndexOf(prepended, item);
                if (result >= 0)
                {
                    return result;
                }
                result = source.IndexOf(item);
                return result >= 0 ? prepended.Length + result : -1;
            }

            public override void CopyTo(TSource[] array, int arrayIndex)
            {
                prepended.CopyTo(array, arrayIndex);
                source.CopyTo(array, arrayIndex + prepended.Length);
            }

            public override IEnumerator<TSource> GetEnumerator() => new IndexerEnumerator(prepended, source);
        }

        private interface IAppendPrepend<TSource> : IAppend<TSource>, IPrepend<TSource>;

        /// <summary>
        /// Represents the insertion of one or more items before or after an <see cref="IReadOnlyList{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the source list.</typeparam>
        private sealed partial class AppendPrependIndexer<TSource>(IReadOnlyList<TSource> source, TSource prepended, TSource appended) : IndexerBase<TSource>, IAppendPrepend<TSource>, ISkip<TSource>, ISkipLast<TSource>, ITake<TSource>, ITakeLast<TSource>, IConcat<TSource>
        {
            public override TSource this[int index] => index == 0 ? prepended : index == source.Count + 1 ? appended : source[index - 1];

            public override int Count => checked(1 + source.Count + 1);

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

            public IReadOnlyList<TSource> Concat(IReadOnlyList<TSource> second) => new ConcatNIndexer<TSource>([prepended], source, [appended], second);

            public override bool Contains(TSource item) => EqualityComparer<TSource>.Default.Equals(prepended, item) || source.Contains(item) || EqualityComparer<TSource>.Default.Equals(appended, item);

            public override int IndexOf(TSource item)
            {
                if (!EqualityComparer<TSource>.Default.Equals(prepended, item))
                {
                    int result = source.IndexOf(item);
                    return result >= 0 ? result + 1 : EqualityComparer<TSource>.Default.Equals(appended, item) ? source.Count + 1 : -1;
                }
                return 0;
            }

            public override void CopyTo(TSource[] array, int arrayIndex)
            {
                array[arrayIndex] = prepended;
                source.CopyTo(array, arrayIndex + 1);
                array[arrayIndex + 1 + source.Count] = appended;
            }

            public override IEnumerator<TSource> GetEnumerator() => new IndexerEnumerator(source, prepended, appended);

            private sealed partial class IndexerEnumerator(IReadOnlyList<TSource> source, TSource prepended, TSource appended) : EnumeratorIndexerEnumeratorBase
            {
                public override bool MoveNext()
                {
                    switch (_index)
                    {
                        case -1:
                            _current = prepended;
                            _index = 0;
                            return true;
                        case 0:
                            _enumerator = source.GetEnumerator();
                            _index = 1;
                            goto case 1;
                        case 1:
                            if (!_enumerator!.MoveNext())
                            {
                                _index = 2;
                                _enumerator.Dispose();
                                _enumerator = null;
                                goto case 2;
                            }
                            _current = _enumerator.Current;
                            return true;
                        case 2:
                            _current = appended;
                            _index = 3;
                            return true;
                        case 3:
                        default:
                            return false;
                    }
                }
            }
        }

        /// <summary>
        /// Represents the insertion of multiple items before or after an <see cref="IReadOnlyList{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the source list.</typeparam>
        private sealed partial class AppendPrependNIndexer<TSource>(IReadOnlyList<TSource> source, TSource[] prepended, TSource[] appended) : IndexerBase<TSource>, IAppendPrepend<TSource>, ISkipBoth<TSource>, ITake<TSource>, ITakeLast<TSource>, IConcat<TSource>
        {
            public override TSource this[int index]
            {
                get
                {
                    int prependedCount, prependedSourceCount;
                    return index < (prependedCount = prepended.Length) ? prepended[index]
                        : index < (prependedSourceCount = prependedCount + source.Count) ? source[index - prependedCount]
                        : appended[index - prependedSourceCount];
                }
            }

            public override int Count => checked(prepended.Length + source.Count + appended.Length);

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

            public IReadOnlyList<TSource> Concat(IReadOnlyList<TSource> second) => new ConcatNIndexer<TSource>(prepended, source, appended, second);

            public override bool Contains(TSource item) => ((ICollection<TSource>)prepended).Contains(item) || source.Contains(item) || ((ICollection<TSource>)appended).Contains(item);

            public override int IndexOf(TSource item)
            {
                int result = Array.IndexOf(prepended, item);
                if (result >= 0)
                {
                    return result;
                }
                result = source.IndexOf(item);
                if (result >= 0)
                {
                    return prepended.Length + result;
                }
                result = Array.IndexOf(appended, item);
                return result >= 0 ? prepended.Length + source.Count + result : -1;
            }

            public override void CopyTo(TSource[] array, int arrayIndex)
            {
                prepended.CopyTo(array, arrayIndex);
                source.CopyTo(array, arrayIndex + prepended.Length);
                appended.CopyTo(array, arrayIndex + prepended.Length + source.Count);
            }

            public override IEnumerator<TSource> GetEnumerator() => new IndexerEnumerator(source, prepended, appended);

            private sealed partial class IndexerEnumerator(IReadOnlyList<TSource> source, IReadOnlyList<TSource> prepended, IReadOnlyList<TSource> appended) : EnumeratorIndexerEnumeratorBase
            {
                public override bool MoveNext()
                {
                    switch (_index)
                    {
                        case -1:
                            _enumerator = prepended.GetEnumerator();
                            _index = 0;
                            goto case 0;
                        case 0:
                            if (!_enumerator!.MoveNext())
                            {
                                _index = 1;
                                _enumerator.Dispose();
                                goto case 1;
                            }
                            _current = _enumerator.Current;
                            return true;
                        case 1:
                            _enumerator = source.GetEnumerator();
                            _index = 2;
                            goto case 2;
                        case 2:
                            if (!_enumerator!.MoveNext())
                            {
                                _index = 3;
                                _enumerator.Dispose();
                                goto case 3;
                            }
                            _current = _enumerator.Current;
                            return true;
                        case 3:
                            _enumerator = appended.GetEnumerator();
                            _index = 4;
                            goto case 4;
                        case 4:
                            if (!_enumerator!.MoveNext())
                            {
                                _index = 5;
                                _enumerator.Dispose();
                                _enumerator = null;
                                goto case 5;
                            }
                            _current = _enumerator.Current;
                            return true;
                        case 5:
                        default:
                            return false;
                    }
                }
            }
        }
    }
}
