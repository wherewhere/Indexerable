using System;
using System.Collections.Generic;
using Xunit;

namespace Indexer.Linq.Tests
{
    public class FirstOrDefaultTests : IndexerableTests
    {
        [Fact]
        public void SameResultsRepeatCallsIntQuery()
        {
            IReadOnlyList<int> ieInt = Indexerable.Range(0, 0);
            IReadOnlyList<int> q = from x in ieInt
                                   select x;

            Assert.Equal(q.FirstOrDefault(), q.FirstOrDefault());
        }

        [Fact]
        public void SameResultsRepeatCallsStringQuery()
        {
            IReadOnlyList<string> q = from x in new[] { "!@#$%^", "C", "AAA", "", "Calling Twice", "SoS", string.Empty }
                                      select x;

            Assert.Equal(q.FirstOrDefault(), q.FirstOrDefault());
        }

        private static void TestEmptyIList<T>()
        {
            T[] source = [];
            T expected = default;

            Assert.IsType<IList<T>>(source, exactMatch: false);

            Assert.Equal(expected, source.FirstOrDefault());
        }

        private static void TestEmptyIListDefault<T>(T defaultValue)
        {
            T[] source = [];

            Assert.IsType<IList<T>>(source, exactMatch: false);

            Assert.Equal(defaultValue, source.FirstOrDefault(defaultValue));
        }

        [Fact]
        public void EmptyIListT()
        {
            TestEmptyIList<int>();
            TestEmptyIList<string>();
            TestEmptyIList<DateTime>();
            TestEmptyIList<FirstOrDefaultTests>();
        }

        [Fact]
        public void EmptyIListDefault()
        {
            TestEmptyIListDefault(5); // int
            TestEmptyIListDefault("Hello"); // string
            TestEmptyIListDefault(DateTime.UnixEpoch); //DateTime
        }

        [Fact]
        public void IListTOneElement()
        {
            int[] source = [5];
            int expected = 5;

            Assert.IsType<IList<int>>(source, exactMatch: false);

            Assert.Equal(expected, source.FirstOrDefault());
        }

        [Fact]
        public void IListOneElementDefault()
        {
            int[] source = [5];
            int expected = 5;

            Assert.IsType<IList<int>>(source, exactMatch: false);

            Assert.Equal(expected, source.FirstOrDefault(3));
        }

        [Fact]
        public void IListTManyElementsFirstIsDefault()
        {
            int?[] source = [null, -10, 2, 4, 3, 0, 2];
            int? expected = null;

            Assert.IsType<IList<int?>>(source, exactMatch: false);

            Assert.Equal(expected, source.FirstOrDefault());
        }

        [Fact]
        public void IListTManyElementsFirstIsNotDefault()
        {
            int?[] source = [19, null, -10, 2, 4, 3, 0, 2];
            int? expected = 19;

            Assert.IsType<IList<int?>>(source, exactMatch: false);

            Assert.Equal(expected, source.FirstOrDefault());
        }

        [Fact]
        public void NullSource()
        {
            AssertExtensions.Throws<ArgumentNullException>("source", () => ((IReadOnlyList<int>)null).FirstOrDefault());
            AssertExtensions.Throws<ArgumentNullException>("source", () => ((IReadOnlyList<int>)null).FirstOrDefault(5));
        }
    }
}
