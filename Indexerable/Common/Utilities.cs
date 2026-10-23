using System;

namespace Indexer.Linq
{
    /// <summary>
    /// Contains helper methods for composing selectors used by <see cref="Indexerable"/>.
    /// </summary>
    internal static class Utilities
    {
        /// <summary>
        /// Combines two selectors.
        /// </summary>
        /// <typeparam name="TSource">The type of the first selector's argument.</typeparam>
        /// <typeparam name="TMiddle">The type of the second selector's argument.</typeparam>
        /// <typeparam name="TResult">The type of the second selector's return value.</typeparam>
        /// <param name="selector1">The first selector to run.</param>
        /// <param name="selector2">The second selector to run.</param>
        /// <returns>A new selector that represents the composition of the first selector with the second selector.</returns>
        public static Func<TSource, TResult> CombineSelectors<TSource, TMiddle, TResult>(this Func<TSource, TMiddle> selector1, Func<TMiddle, TResult> selector2) =>
            x => selector2(selector1(x));

        /// <summary>
        /// Combines two selectors.
        /// </summary>
        /// <typeparam name="TSource">The type of the first selector's argument.</typeparam>
        /// <typeparam name="TMiddle">The type of the second selector's argument.</typeparam>
        /// <typeparam name="TResult">The type of the second selector's return value.</typeparam>
        /// <param name="selector1">The first selector to run.</param>
        /// <param name="selector2">The second selector to run.</param>
        /// <returns>A new selector that represents the composition of the first selector with the second selector.</returns>
        public static Func<TSource, int, TResult> CombineSelectors<TSource, TMiddle, TResult>(this Func<TSource, int, TMiddle> selector1, Func<TMiddle, TResult> selector2) =>
            (x, i) => selector2(selector1(x, i));

        /// <summary>
        /// Combines two selectors.
        /// </summary>
        /// <typeparam name="TSource">The type of the first selector's argument.</typeparam>
        /// <typeparam name="TMiddle">The type of the second selector's argument.</typeparam>
        /// <typeparam name="TResult">The type of the second selector's return value.</typeparam>
        /// <param name="selector1">The first selector to run.</param>
        /// <param name="selector2">The second selector to run.</param>
        /// <returns>A new selector that represents the composition of the first selector with the second selector.</returns>
        public static Func<TSource, int, TResult> CombineSelectors<TSource, TMiddle, TResult>(this Func<TSource, int, TMiddle> selector1, Func<TMiddle, int, TResult> selector2) =>
            (x, i) => selector2(selector1(x, i), i);


        /// <summary>
        /// Combines two selectors.
        /// </summary>
        /// <typeparam name="TSource">The type of the second selector's argument.</typeparam>
        /// <typeparam name="TResult">The type of the second selector's return value.</typeparam>
        /// <param name="selector1">The first selector to run.</param>
        /// <param name="selector2">The second selector to run.</param>
        /// <returns>A new selector that represents the composition of the first selector with the second selector.</returns>
        public static Func<int, TResult> CombineSelectors<TSource, TResult>(this Func<int, TSource> selector1, Func<TSource, TResult> selector2) =>
            i => selector2(selector1(i));

        /// <summary>
        /// Combines two selectors.
        /// </summary>
        /// <typeparam name="TSource">The type of the second selector's argument.</typeparam>
        /// <typeparam name="TResult">The type of the second selector's return value.</typeparam>
        /// <param name="selector1">The first selector to run.</param>
        /// <param name="selector2">The second selector to run.</param>
        /// <returns>A new selector that represents the composition of the first selector with the second selector.</returns>
        public static Func<int, TResult> CombineSelectors<TSource, TResult>(this Func<int, TSource> selector1, Func<TSource, int, TResult> selector2) =>
            i => selector2(selector1(i), i);

        /// <summary>
        /// Combines two selectors.
        /// </summary>
        /// <typeparam name="TSource">The type of the second selector's argument.</typeparam>
        /// <typeparam name="TResult">The type of the second selector's return value.</typeparam>
        /// <param name="selector1">The first selector to run.</param>
        /// <param name="selector2">The second selector to run.</param>
        /// <param name="offset">The offset to apply to the index.</param>
        /// <returns>A new selector that represents the composition of the first selector with the second selector.</returns>
        public static Func<int, TResult> CombineSelectors<TSource, TResult>(this Func<int, TSource> selector1, Func<TSource, int, TResult> selector2, int offset) =>
            i => selector2(selector1(i), i - offset);
    }
}
