#pragma warning disable CS0618
using System;
using System.Collections.Generic;
using Xunit;

namespace Indexer.Linq.Tests
{
    public class CountTests : IndexerableTests
    {
        [Fact]
        public void SameResultsRepeatCallsIntQuery()
        {
            IReadOnlyList<int> q = from x in new[] { 9999, 0, 888, -1, 66, -777, 1, 2, -12345 }
                                   select x;

            Assert.Equal(q.Count(), q.Count());
        }

        [Fact]
        public void SameResultsRepeatCallsStringQuery()
        {
            IReadOnlyList<string> q = from x in new[] { "!@#$%^", "C", "AAA", "", "Calling Twice", "SoS", string.Empty }
                                      select x;

            Assert.Equal(q.Count(), q.Count());
        }

        [Fact]
        public void NullableIntArray_IncludesNullObjects()
        {
            int?[] data = [-10, 4, 9, null, 11];
            Assert.Equal(5, data.Count());
        }

        [Theory]
        [MemberData(nameof(CountsAndTallies))]
        public void CountMatchesTally<T>(int count, IReadOnlyList<T> indexerable)
        {
            Assert.Equal(count, indexerable.Count());
        }

        [Theory, MemberData(nameof(CountsAndTallies))]
        public void RunOnce<T>(int count, IReadOnlyList<T> indexerable)
        {
            Assert.Equal(count, indexerable.Count());
        }

        private static IEnumerable<object[]> EnumerateCollectionTypesAndCounts<T>(int count, IReadOnlyList<T> indexerable)
        {
            foreach (IReadOnlyList<T> source in CreateSources(indexerable))
            {
                yield return [count, source];
            }
        }

        public static IEnumerable<object[]> CountsAndTallies()
        {
            int count = 5;
            IReadOnlyList<int> range = Indexerable.Range(1, count);
            foreach (object[] variant in EnumerateCollectionTypesAndCounts(count, range))
            {
                yield return variant;
            }

            foreach (object[] variant in EnumerateCollectionTypesAndCounts(count, range.Select(i => (float)i)))
            {
                yield return variant;
            }

            foreach (object[] variant in EnumerateCollectionTypesAndCounts(count, range.Select(i => (double)i)))
            {
                yield return variant;
            }

            foreach (object[] variant in EnumerateCollectionTypesAndCounts(count, range.Select(i => (decimal)i)))
            {
                yield return variant;
            }
        }

        [Fact]
        public void NullSource_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("source", () => ((IReadOnlyList<int>)null).Count());
        }
    }
}
