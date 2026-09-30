using System;
using Xunit;
using Xunit.Sdk;

namespace Indexer
{
    public static class AssertExtensions
    {
        public static T Throws<T>(string expectedParamName, Action action)
            where T : ArgumentException
        {
            T exception = Assert.Throws<T>(action);

            Assert.Equal(expectedParamName, exception.ParamName);

            return exception;
        }

        /// <summary>
        /// Validates that the actual span is equal to the expected span.
        /// If this fails, determine where the differences are and create an exception with that information.
        /// </summary>
        /// <param name="expected">The array that <paramref name="actual"/> should be equal to.</param>
        /// <param name="actual"></param>
        public static void SequenceEqual<T>(ReadOnlySpan<T> expected, ReadOnlySpan<T> actual) where T : IEquatable<T>
        {
            // Use the SequenceEqual to compare the arrays for better performance. The default Assert.Equal method compares
            // the arrays by boxing each element that is very slow for large arrays.
            if (!expected.SequenceEqual(actual))
            {
                if (expected.Length != actual.Length)
                {
                    throw new XunitException($"Expected: Span of length {expected.Length}{Environment.NewLine}Actual: Span of length {actual.Length}");
                }
                else
                {
                    const int MaxDiffsToShow = 10;      // arbitrary; enough to be useful, hopefully, but still manageable

                    int diffCount = 0;
                    string message = $"Showing first {MaxDiffsToShow} differences{Environment.NewLine}";
                    for (int i = 0; i < expected.Length; i++)
                    {
                        if (!expected[i].Equals(actual[i]))
                        {
                            diffCount++;

                            // Add up to 10 differences to the exception message
                            if (diffCount <= MaxDiffsToShow)
                            {
                                message += $"  Position {i}: Expected: {expected[i]}, Actual: {actual[i]}{Environment.NewLine}";
                            }
                        }
                    }

                    message += $"Total number of differences: {diffCount} out of {expected.Length}";

                    throw new XunitException(message);
                }
            }
        }
    }
}
