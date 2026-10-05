using System;
using System.Collections.Generic;
using System.Linq;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        /// <summary>
        /// Concatenates two read-only lists.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input lists.</typeparam>
        /// <param name="first">The first list to concatenate.</param>
        /// <param name="second">The list to concatenate after <paramref name="first"/>.</param>
        /// <returns>A read-only list that contains the elements of <paramref name="first"/> followed by the elements of <paramref name="second"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="first"/> or <paramref name="second"/> is <see langword="null"/>.</exception>
        public static IReadOnlyList<TSource> Concat<TSource>(this IReadOnlyList<TSource> first, IReadOnlyList<TSource> second)
        {
            if (first is null) { ThrowHelper.ThrowArgumentNullException(nameof(first)); }
            if (second is null) { ThrowHelper.ThrowArgumentNullException(nameof(second)); }
            return new ConcatIndexer<TSource>(first, second);
        }

        private interface IConcat<TSource> : IReadOnlyList<TSource>
        {
            /// <inheritdoc cref="Concat{TSource}(IReadOnlyList{TSource}, IReadOnlyList{TSource})"/>
            IReadOnlyList<TSource> Concat(IReadOnlyList<TSource> second);
        }

        private abstract partial class ConcatIndexerBase<TSource> : IndexerBase<TSource>
        {
            protected sealed partial class IndexerEnumerator(IReadOnlyList<TSource> source, IReadOnlyList<TSource> second) : EnumeratorIndexerEnumeratorBase
            {
                public override bool MoveNext()
                {
                    switch (_index)
                    {
                        case -1:
                        case 0:
                            _enumerator = source.GetEnumerator();
                            _index = 1;
                            goto case 1;
                        case 1:
                            if (!_enumerator!.MoveNext())
                            {
                                _index = 2;
                                _enumerator.Dispose();
                                goto case 2;
                            }
                            _current = _enumerator.Current;
                            return true;
                        case 2:
                            _enumerator = second.GetEnumerator();
                            _index = 3;
                            goto case 3;
                        case 3:
                            if (!_enumerator!.MoveNext())
                            {
                                _index = 4;
                                _enumerator.Dispose();
                                _enumerator = null;
                                goto case 4;
                            }
                            _current = _enumerator.Current;
                            return true;
                        case 4:
                        default:
                            return false;
                    }
                }
            }
        }

        /// <summary>
        /// Represents the concatenation of two <see cref="IReadOnlyList{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the source lists.</typeparam>
        private sealed partial class ConcatIndexer<TSource>(IReadOnlyList<TSource> first, IReadOnlyList<TSource> second) : ConcatIndexerBase<TSource>, IConcat<TSource>
        {
            public override TSource this[int index]
            {
                get
                {
                    int firstCount;
                    return index < (firstCount = first.Count) ? first[index] : second[index - firstCount];
                }
            }

            public override int Count => first.Count + second.Count;

            public IReadOnlyList<TSource> Concat(IReadOnlyList<TSource> _second) => new ConcatNIndexer<TSource>(first, second, _second);

            public override bool Contains(TSource item) => first.Contains(item) || second.Contains(item);

            public override int IndexOf(TSource item)
            {
                int result = first.IndexOf(item);
                if (result >= 0)
                {
                    return result;
                }
                result = second.IndexOf(item);
                return result >= 0 ? first.Count + result : -1;
            }

            public override void CopyTo(TSource[] array, int arrayIndex)
            {
                first.CopyTo(array, arrayIndex);
                second.CopyTo(array, arrayIndex + first.Count);
            }

            public override IEnumerator<TSource> GetEnumerator() => new IndexerEnumerator(first, second);
        }

        /// <summary>
        /// Represents the concatenation of three or more <see cref="IReadOnlyList{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the source lists.</typeparam>
        /// <remarks>Chained concatenations retain their source lists and enumerate each source in order.</remarks>
        private sealed partial class ConcatNIndexer<TSource>(IReadOnlyList<TSource> first, params IReadOnlyList<TSource>[] rest) : IndexerBase<TSource>, IConcat<TSource>
        {
            public override TSource this[int index]
            {
                get
                {
                    int offset;
                    if (index < (offset = first.Count))
                    {
                        return first[index];
                    }
                    int temp = offset;
                    foreach (IReadOnlyList<TSource> source in rest)
                    {
                        if (index < (temp += source.Count))
                        {
                            return source[index - offset];
                        }
                        offset = temp;
                    }
                    throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
                }
            }

            public override int Count
            {
                get
                {
                    return checked(first.Count + rest.Sum(Count));
                    static int Count(IReadOnlyList<TSource> source) => source.Count;
                }
            }

            public IReadOnlyList<TSource> Concat(IReadOnlyList<TSource> _second) => new ConcatNIndexer<TSource>(first, [.. rest, _second]);

            public override bool Contains(TSource item) => first.Contains(item) || rest.Any(x => x.Contains(item));

            public override int IndexOf(TSource item)
            {
                int result = first.IndexOf(item);
                if (result >= 0)
                {
                    return result;
                }
                int index = first.Count;
                foreach (IReadOnlyList<TSource> source in rest)
                {
                    result = source.IndexOf(item);
                    if (result >= 0)
                    {
                        return index + result;
                    }
                    index += source.Count;
                }
                return -1;
            }

            public override void CopyTo(TSource[] array, int arrayIndex)
            {
                first.CopyTo(array, arrayIndex);
                int index = first.Count;
                foreach (IReadOnlyList<TSource> source in rest)
                {
                    source.CopyTo(array, arrayIndex + index);
                    index += source.Count;
                }
            }

            public override IEnumerator<TSource> GetEnumerator() => new IndexerEnumerator(first, rest);

            private sealed partial class IndexerEnumerator(IReadOnlyList<TSource> first, params IReadOnlyList<TSource>[] rest) : EnumeratorIndexerEnumeratorBase
            {
                private int _status = -1;

                public override bool MoveNext()
                {
                    switch (_index)
                    {
                        case -1:
                        case 0:
                            _enumerator = first.GetEnumerator();
                            _index = 1;
                            goto case 1;
                        case 1:
                            if (!_enumerator!.MoveNext())
                            {
                                _index = 2;
                                _enumerator.Dispose();
                                goto case 2;
                            }
                            _current = _enumerator.Current;
                            return true;
                        case 2:
                            int index = _status + 1, count = rest.Length;
                            if ((uint)index < (uint)count)
                            {
                                _enumerator = rest[_status = index].GetEnumerator();
                                _index = 3;
                                goto case 3;
                            }
                            _status = count;
                            goto case 4;
                        case 3:
                            if (!_enumerator!.MoveNext())
                            {
                                _index = 2;
                                _enumerator.Dispose();
                                goto case 2;
                            }
                            _current = _enumerator.Current;
                            return true;
                        case 4:
                        default:
                            return false;
                    }
                }
            }
        }
    }
}
