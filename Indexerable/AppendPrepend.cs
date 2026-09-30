using System;
using System.Collections.Generic;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        public static IReadOnlyList<TSource> Append<TSource>(this IReadOnlyList<TSource> source, TSource element)
        {
            ArgumentNullException.ThrowIfNull(source);
            return new AppendIndexer<TSource>(source, element);
        }

        /// <summary>
        /// Represents the insertion of one or more items after an <see cref="IReadOnlyList{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the source enumerable.</typeparam>
        private sealed partial class AppendIndexer<TSource>(IReadOnlyList<TSource> source, TSource element) : Indexer<TSource>
        {
            public override TSource this[int index] => index == source.Count ? element : source[index];
            public override int Count => checked(source.Count + 1);
        }

        public static IReadOnlyList<TSource> Prepend<TSource>(this IReadOnlyList<TSource> source, TSource element)
        {
            ArgumentNullException.ThrowIfNull(source);
            return new PrependIndexer<TSource>(source, element);
        }

        /// <summary>
        /// Represents the insertion of one or more items before an <see cref="IReadOnlyList{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the source enumerable.</typeparam>
        private sealed partial class PrependIndexer<TSource>(IReadOnlyList<TSource> source, TSource element) : Indexer<TSource>
        {
            public override TSource this[int index] => index == 0 ? element : source[index - 1];
            public override int Count => checked(source.Count + 1);
        }
    }
}
