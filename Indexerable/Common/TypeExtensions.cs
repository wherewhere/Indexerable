#if !NETSTANDARD || !NETSTANDARD1_3_OR_GREATER
global using static Indexer.Linq.TypeExtensions;
using System;

namespace Indexer.Linq
{
    /// <summary>
    /// Provides extension methods for the <see cref="Type"/> class.
    /// </summary>
    internal static class TypeExtensions
    {
        /// <summary>
        /// A extension for <see cref="Type"/>.
        /// </summary>
        extension(Type)
        {
            /// <summary>
            /// Represents an empty array of type <see cref="Type"/>. This field is read-only.
            /// </summary>
            public static Type[] EmptyTypes => [];
        }
    }
}
#endif