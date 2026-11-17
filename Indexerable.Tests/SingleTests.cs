using System;
using System.Collections.Generic;
using Xunit;

namespace Indexer.Linq.Tests
{
    public class SingleTests : IndexerableTests
    {
        [Fact]
        public void SameResultsRepeatCallsIntQuery()
        {
            IReadOnlyList<decimal> q = from x in (decimal[])[999.9m]
                                       select x;

            Assert.Equal(q.Single(), q.Single());
        }

        [Fact]
        public void SameResultsRepeatCallsStringQuery()
        {
            IReadOnlyList<string> q = from x in (string[])["!@#$%^"]
                                      select x;

            Assert.Equal(q.Single(), q.Single());
        }

        [Fact]
        public void SameResultsRepeatCallsIntQueryWithZero()
        {
            IReadOnlyList<int> q = from x in (int[])[0]
                                   select x;

            Assert.Equal(q.Single(), q.Single());
        }

        [Fact]
        public void Empty()
        {
            foreach (IReadOnlyList<int> source in CreateSources<int>([]))
            {
                Assert.Throws<InvalidOperationException>(() => source.Single());
            }
        }

        [Fact]
        public void SingleElement()
        {
            int expected = 4;
            foreach (IReadOnlyList<int> source in CreateSources([4]))
            {
                Assert.Equal(expected, source.Single());
            }
        }

        [Fact]
        public void ManyElement()
        {
            foreach (IReadOnlyList<int> source in CreateSources([4, 4, 4, 4, 4]))
            {
                Assert.Throws<InvalidOperationException>(() => source.Single());
            }
        }

        [Fact]
        public void ThrowsOnNullSource()
        {
            int[] source = null;
            AssertExtensions.Throws<ArgumentNullException>("source", () => source.Single());
        }
    }
}
