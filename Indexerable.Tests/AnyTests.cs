#pragma warning disable CS0618
using System;
using System.Collections.Generic;
using Xunit;

namespace Indexer.Linq.Tests
{
    public class AnyTests : IndexerableTests
    {
        public static IEnumerable<TheoryDataRow<IReadOnlyList<int>, bool>> TestData()
        {
            foreach (int count in new[] { 0, 1, 2 })
            {
                bool expected = count > 0;
                foreach (IReadOnlyList<int> source in CreateSources(new int[count]))
                {
                    yield return new(source, expected);
                    yield return new(source.Select(i => i), expected);
                }
            }
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void Any(IReadOnlyList<int> source, bool expected)
        {
            Assert.Equal(expected, source.Any());
        }

        [Fact]
        public void NullObjectsInArray_Included()
        {
            int?[] source = [null, null, null, null];
            Assert.All(CreateSources(source), source => Assert.True(source.Any()));
        }

        [Fact]
        public void NullSource_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("source", () => ((IReadOnlyList<int>)null).Any());
        }
    }
}
