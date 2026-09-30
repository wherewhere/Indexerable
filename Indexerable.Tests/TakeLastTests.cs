using System;
using System.Collections.Generic;
using Xunit;

namespace Indexer.Linq.Tests
{
    public class TakeLastTests : IndexerableTests
    {
        [Fact]
        public void SkipLastThrowsOnNull()
        {
            AssertExtensions.Throws<ArgumentNullException>("source", () => ((IReadOnlyList<int>)null).TakeLast(10));
        }

        [Theory]
        [MemberData(nameof(SkipTakeData.IndexerableData), MemberType = typeof(SkipTakeData))]
        public void TakeLast(IReadOnlyList<int> source, int count)
        {
            IReadOnlyList<int> expected = source.Reverse().Take(count).Reverse();

            Assert.All(CreateSources(source), source =>
            {
                IReadOnlyList<int> actual = source.TakeLast(count);

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
        public void List_ChangesAfterTakeLast_ChangesReflectedInResults()
        {
            List<int> list = [1, 2, 3, 4, 5];

            IReadOnlyList<int> e = list.TakeLast(3);

            list.RemoveAt(0);
            list.RemoveAt(0);

            Assert.Equal([3, 4, 5], e);
        }

        [Fact]
        public void List_Skip_ChangesAfterTakeLast_ChangesReflectedInResults()
        {
            List<int> list = [1, 2, 3, 4, 5];

            IReadOnlyList<int> e = list.Skip(1).TakeLast(3);

            list.RemoveAt(0);

            Assert.Equal([3, 4, 5], e);
        }
    }
}
