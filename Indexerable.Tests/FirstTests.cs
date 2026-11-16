using System;
using System.Collections.Generic;
using Xunit;

namespace Indexer.Linq.Tests
{
    public class FirstTests : IndexerableTests
    {
        [Fact]
        public void SameResultsRepeatCallsIntQuery()
        {
            IReadOnlyList<int> q = from x in new[] { 9999, 0, 888, -1, 66, -777, 1, 2, -12345 }
                                   select x;

            Assert.Equal(q.First(), q.First());
        }

        [Fact]
        public void SameResultsRepeatCallsStringQuery()
        {
            IReadOnlyList<string> q = from x in new[] { "!@#$%^", "C", "AAA", "", "Calling Twice", "SoS", string.Empty }
                                      select x;

            Assert.Equal(q.First(), q.First());
        }

        private static void TestEmptyIList<T>()
        {
            T[] source = [];

            Assert.NotNull(source);

            Assert.Throws<InvalidOperationException>(() => source.First());
        }

        [Fact]
        public void EmptyIListT()
        {
            TestEmptyIList<int>();
            TestEmptyIList<string>();
            TestEmptyIList<DateTime>();
            TestEmptyIList<FirstTests>();
        }

        [Fact]
        public void IListTOneElement()
        {
            int[] source = [5];
            int expected = 5;

            Assert.NotNull(source);

            Assert.Equal(expected, source.First());
        }

        [Fact]
        public void IListTManyElementsFirstIsDefault()
        {
            int?[] source = [null, -10, 2, 4, 3, 0, 2];
            int? expected = null;

            Assert.IsType<IList<int?>>(source, exactMatch: false);

            Assert.Equal(expected, source.First());
        }

        [Fact]
        public void IListTManyElementsFirstIsNotDefault()
        {
            int?[] source = [19, null, -10, 2, 4, 3, 0, 2];
            int? expected = 19;

            Assert.IsType<IList<int?>>(source, exactMatch: false);

            Assert.Equal(expected, source.First());
        }

        [Fact]
        public void NullSource()
        {
            AssertExtensions.Throws<ArgumentNullException>("source", () => ((IReadOnlyList<int>)null).First());
        }
    }
}
