using System;
using System.Collections.Generic;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        /// <summary>
        /// Determines whether a sequence contains any elements.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence to check for being empty.</param>
        /// <returns><see langword="true"/> if <paramref name="source"/> contains any elements; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        [Obsolete("Use source.Length/Count > 0 property instead of Indexerable.Any(source) method.")]
        public static bool Any<TSource>(this IReadOnlyList<TSource> source)
        {
            if (source is null) { ThrowHelper.ThrowArgumentNullException(nameof(source)); }
            return source.Count > 0;
        }
    }
}
