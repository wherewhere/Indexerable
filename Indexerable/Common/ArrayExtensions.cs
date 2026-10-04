#if !NETCOREAPP2_0_OR_GREATER || !COMP_NETSTANDARD2_1
using System;

namespace Indexer.Linq
{
    /// <summary>
    /// Provides compatibility methods for the <see cref="Array"/> class.
    /// </summary>
    internal static class ArrayExtensions
    {
        /// <summary>
        /// A extension for <see cref="Array"/>.
        /// </summary>
        extension(Array)
        {
            /// <summary>
            /// Assigns the specified <paramref name="value"/> to the elements of the specified
            /// <paramref name="array"/> that are within the range of <paramref name="startIndex"/>
            /// (inclusive) and the next <paramref name="count"/> number of indices.
            /// </summary>
            /// <typeparam name="T">The type of the elements of the array.</typeparam>
            /// <param name="array">The array to be filled.</param>
            /// <param name="value">The new value for the elements in the specified range.</param>
            /// <param name="startIndex">A 32-bit integer that represents the index in <paramref name="array"/> at which filling begins.</param>
            /// <param name="count">The number of elements to fill.</param>
            /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
            /// <exception cref="ArgumentOutOfRangeException">
            /// <paramref name="startIndex"/> is outside the array bounds, or <paramref name="count"/> is negative
            /// or exceeds the number of elements from <paramref name="startIndex"/> to the end of the array.
            /// </exception>
            public static void Fill<T>(T[] array, T value, int startIndex, int count)
            {
                if (array == null)
                {
                    ThrowHelper.ThrowArgumentNullException(nameof(array));
                }

                if ((uint)startIndex > (uint)array.Length)
                {
                    ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqual(nameof(startIndex));
                }

                if ((uint)count > (uint)(array.Length - startIndex))
                {
                    ThrowHelper.ThrowArgumentOutOfRange_Count(nameof(count));
                }

                for (int i = startIndex; i < startIndex + count; i++)
                {
                    array[i] = value;
                }
            }
        }
    }
}
#endif