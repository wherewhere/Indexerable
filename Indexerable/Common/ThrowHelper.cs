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
    }
}
