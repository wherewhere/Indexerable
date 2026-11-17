using System;
using System.Collections.Generic;
using Xunit;

namespace Indexer.Linq.Tests
{
    public class SingleOrDefaultTests : IndexerableTests
    {
        [Fact]
        public void SameResultsRepeatCallsIntQuery()
        {
            IReadOnlyList<float> q = from x in (float[])[0.12335f]
                                     select x;

            Assert.Equal(q.SingleOrDefault(), q.SingleOrDefault());
        }

        [Fact]
        public void Empty()
        {
            foreach (IReadOnlyList<int?> source in CreateSources<int?>([]))
            {
                Assert.Null(source.SingleOrDefault());
                Assert.Equal(5, source.SingleOrDefault(5));
            }
        }

        [Fact]
        public void SingleElement()
        {
            foreach (IReadOnlyList<int?> source in CreateSources<int?>([4]))
            {
                Assert.Equal(4, source.SingleOrDefault());
                Assert.Equal(4, source.SingleOrDefault(5));
            }
        }

        [Fact]
        public void ManyElementIList()
        {
            foreach (IReadOnlyList<int?> source in CreateSources<int?>([4, 4, 4, 4, 4]))
            {
                Assert.Throws<InvalidOperationException>(() => source.SingleOrDefault());
                Assert.Throws<InvalidOperationException>(() => source.SingleOrDefault(4));
            }
        }

        [Fact]
        public void ThrowsOnNullSource()
        {
            int[] source = null;
            AssertExtensions.Throws<ArgumentNullException>("source", () => source.SingleOrDefault());
        }

        [Fact]
        public void ThrowsOnNullSourceDefault()
        {
            int[] source = null;
            AssertExtensions.Throws<ArgumentNullException>("source", () => source.SingleOrDefault(5));
        }
    }
}
