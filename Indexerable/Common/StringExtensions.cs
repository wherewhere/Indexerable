#if !NETCOREAPP2_1_OR_GREATER || COMP_NETSTANDARD2_1
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Indexer.Linq
{
    /// <summary>
    /// Provides extension methods for the <see cref="string"/> class.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class StringExtensions
    {
        /// <summary>
        /// Returns a value indicating whether a specified character occurs within this string.
        /// </summary>
        /// <param name="text">A sequence in which to locate a value.</param>
        /// <param name="value">The character to seek.</param>
        /// <returns><see langword="true"/> if the <paramref name="value"/> parameter occurs within this string; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this string text, char value) => text.IndexOf(value) >= 0;
    }
}
#endif