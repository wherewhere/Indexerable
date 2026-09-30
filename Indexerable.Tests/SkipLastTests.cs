using System;
using System.Collections.Generic;
using Xunit;

namespace Indexer.Linq.Tests
{
    public class SkipLastTests : IndexerableTests
    {
        [Fact]
        public void SkipLastThrowsOnNull()
        {
            AssertExtensions.Throws<ArgumentNullException>("source", () => ((IReadOnlyList<int>)null).SkipLast(10));
        }

        [Theory]
        [MemberData(nameof(SkipTakeData.IndexerableData), MemberType = typeof(SkipTakeData))]
        public void SkipLast(IReadOnlyList<int> source, int count)
        {
            IReadOnlyList<int> expected = source.Reverse().Skip(count).Reverse();

            Assert.All(CreateSources(source), source =>
            {
                IReadOnlyList<int> actual = source.SkipLast(count);

                Assert.Equal(expected, actual);

                Assert.Equal(expected.Count, actual.Count);

                if (expected.Count > 0)
                {
                    Assert.Equal(expected[0], actual[0]);
                    Assert.Equal(expected[^1], actual[^1]);
                    Assert.Equal(expected[expected.Count / 2], actual[expected.Count / 2]);
                }
            });
        }

        [Fact]
        public void List_ChangesAfterSkipLast_ChangesReflectedInResults()
        {
            List<int> list = [1, 2, 3, 4, 5];

            IReadOnlyList<int> e = list.SkipLast(2);

            list.RemoveAt(4);
            list.RemoveAt(3);

            Assert.Equal([1], e);
        }

        [Fact]
        public void List_Skip_ChangesAfterSkipLast_ChangesReflectedInResults()
        {
            List<int> list = [1, 2, 3, 4, 5];

            IReadOnlyList<int> e = list.Skip(1).SkipLast(2);

            list.RemoveAt(4);

            Assert.Equal([2], e);
        }
    }
}
