#if !COMP_NETSTANDARD2_1
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Indexer.Linq;

namespace System.Runtime.CompilerServices
{
    internal static class RuntimeHelpers
    {
        /// <summary>
        /// Slices the specified array using the specified range.
        /// </summary>
        /// <summary>
        /// Slices the specified array using the specified range.
        /// </summary>
        public static T[] GetSubArray<T>(T[] array, Range range)
        {
            if (array is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(array));
            }

            (int offset, int length) = range.GetOffsetAndLength(array.Length);

            if (default(T) != null || typeof(T[]) == array.GetType())
            {
                // We know the type of the array to be exactly T[].

                if (length == 0)
                {
                    return [];
                }

                T[] dest = new T[length];
                Array.Copy(array, offset, dest, 0, length);
                return dest;
            }
            else
            {
                // The array is actually a U[] where U:T.
                T[] dest = (T[])Array.CreateInstance(array.GetType().GetElementType(), length);
                Array.Copy(array, offset, dest, 0, length);
                return dest;
            }
        }

#if !HAS_VALUETUPLE
        private static void Deconstruct<T1, T2>(this Tuple<T1, T2> range, out T1 item1, out T2 item2)
        {
            item1 = range.Item1;
            item2 = range.Item2;
        }
#endif
    }
}
#endif