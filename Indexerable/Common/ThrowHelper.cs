using System;
using System.Diagnostics.CodeAnalysis;

namespace Indexer.Linq
{
    internal static class ThrowHelper
    {
        [DoesNotReturn]
        internal static void ThrowArgumentNullException(string argument) => throw new ArgumentNullException(argument);
        [DoesNotReturn]
        internal static void ThrowArgumentOutOfRangeException(string name) => throw new ArgumentOutOfRangeException(name);
        [DoesNotReturn]
        internal static void ThrowArgumentOutOfRange_IndexMustBeLess(string name) => throw new ArgumentOutOfRangeException(name, "Index was out of range. Must be non-negative and less than the size of the collection.");
#if !NETCOREAPP2_0_OR_GREATER || !COMP_NETSTANDARD2_1
        [DoesNotReturn]
        internal static void ThrowArgumentOutOfRange_IndexMustBeLessOrEqual(string name) => throw new ArgumentOutOfRangeException(name, "Index was out of range. Must be non-negative and less than or equal to the size of the collection.");
        [DoesNotReturn]
        internal static void ThrowArgumentOutOfRange_Count(string name) => throw new ArgumentOutOfRangeException(name, "Count must be positive and count must refer to a location within the string/array/collection.");
#endif
    }
}
