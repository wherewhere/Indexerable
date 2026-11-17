using System;
using System.Collections.Generic;

namespace Indexer.Linq
{
    public static partial class Indexerable
    {
        /// <summary>
        /// Returns the element at a specified index in a sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence to return an element from.</param>
        /// <param name="index">The zero-based index of the element to retrieve.</param>
        /// <returns>The element at the specified position in <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero or greater than or equal to the number of elements in <paramref name="source"/>.</exception>
        [Obsolete("Use source[index] index instead of Indexerable.ElementAt(source, index) method.")]
        public static TSource ElementAt<TSource>(this IReadOnlyList<TSource> source, int index)
        {
            if (source is null) { ThrowHelper.ThrowArgumentNullException(nameof(source)); }
            return source[index];
        }

        /// <summary>
        /// Returns the element at a specified index in a sequence or a default value if the index is out of range.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence to return an element from.</param>
        /// <param name="index">The zero-based index of the element to retrieve.</param>
        /// <returns><see langword="default"/> if <paramref name="index"/> is outside the bounds of <paramref name="source"/>; otherwise, the element at the specified position in <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        public static TSource? ElementAtOrDefault<TSource>(this IReadOnlyList<TSource> source, int index)
        {
            if (source is null) { ThrowHelper.ThrowArgumentNullException(nameof(source)); }
            return (uint)index < (uint)source.Count ? source[index] : default;
        }

#if COMP_NETSTANDARD2_1
        /// <summary>
        /// Returns the element at a specified index in a sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence to return an element from.</param>
        /// <param name="index">The index of the element to retrieve, which is either from the start or the end.</param>
        /// <returns>The element at the specified position in <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is outside the bounds of <paramref name="source"/>.</exception>
        [Obsolete("Use source[index] index instead of Indexerable.ElementAt(source, index) method.")]
        public static TSource ElementAt<TSource>(this IReadOnlyList<TSource> source, Index index)
        {
            if (source is null) { ThrowHelper.ThrowArgumentNullException(nameof(source)); }
            return source[index];
        }

        /// <summary>
        /// Returns the element at a specified index in a sequence or a default value if the index is out of range.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence to return an element from.</param>
        /// <param name="index">The index of the element to retrieve, which is either from the start or the end.</param>
        /// <returns><see langword="default"/> if <paramref name="index"/> is outside the bounds of <paramref name="source"/>; otherwise, the element at the specified position in <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// The default value for reference and nullable types is <see langword="null"/>.
        /// </remarks>
        public static TSource? ElementAtOrDefault<TSource>(this IReadOnlyList<TSource> source, Index index)
        {
            if (source is null) { ThrowHelper.ThrowArgumentNullException(nameof(source)); }
            int count = source.Count;
            int offset = index.GetOffset(count);
            return (uint)offset < (uint)count ? source[offset] : default;
        }
#endif
    }
}
