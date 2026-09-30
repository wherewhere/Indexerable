using System;
using System.Diagnostics.CodeAnalysis;

namespace Indexer.Linq
{
    internal static class ThrowHelper
    {
        [DoesNotReturn]
        public static void ThrowArgumentOutOfRangeException(string name) => throw new ArgumentOutOfRangeException(name);
    }
}
