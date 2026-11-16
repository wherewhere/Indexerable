using System;
using System.Collections.Generic;
using Xunit;

namespace Indexer.Linq.Tests
{
    public class LastOrDefaultTests : IndexerableTests
    {
        [Fact]
        public void SameResultsRepeatCallsIntQuery()
        {
            IReadOnlyList<int> q = from x in new[] { 9999, 0, 888, -1, 66, -777, 1, 2, -12345 }
                                   select x;

            Assert.Equal(q.LastOrDefault(), q.LastOrDefault());
        }

        [Fact]
        public void SameResultsRepeatCallsStringQuery()
        {
            IReadOnlyList<string> q = from x in new[] { "!@#$%^", "C", "AAA", "", "Calling Twice", "SoS", string.Empty }
                                      select x;

            Assert.Equal(q.LastOrDefault(), q.LastOrDefault());
        }

        private static void TestEmptyIList<T>()
        {
            T[] source = [];
            T expected = default;

            Assert.IsType<IList<T>>(source, exactMatch: false);

            Assert.Equal(expected, source.LastOrDefault());
        }

        private static void TestEmptyIListDefault<T>(T defaultValue)
        {
            T[] source = [];

            Assert.IsType<IList<T>>(source, exactMatch: false);

            Assert.Equal(defaultValue, source.LastOrDefault(defaultValue));
        }

        [Fact]
        public void EmptyIListT()
        {
            TestEmptyIList<int>();
            TestEmptyIList<string>();
            TestEmptyIList<DateTime>();
            TestEmptyIList<LastOrDefaultTests>();
        }

        [Fact]
        public void EmptyIList()
        {
            TestEmptyIListDefault(5); // int
            TestEmptyIListDefault("Hello"); // string
            TestEmptyIListDefault(DateTime.UnixEpoch);
        }

        [Fact]
        public void IListTOneElement()
        {
            int[] source = [5];
            int expected = 5;

            Assert.IsType<IList<int>>(source, exactMatch: false);

            Assert.Equal(expected, source.LastOrDefault());
        }

        [Fact]
        public void IListTOneElementDefault()
        {
            int[] source = [5];
            int expected = 5;

            Assert.IsType<IList<int>>(source, exactMatch: false);

            Assert.Equal(expected, source.LastOrDefault(4));
        }


        [Fact]
        public void IListTManyElementsLastIsDefault()
        {
            int?[] source = [-10, 2, 4, 3, 0, 2, null];
            int? expected = null;

            Assert.IsType<IList<int?>>(source, exactMatch: false);

            Assert.Equal(expected, source.LastOrDefault());
        }

        [Fact]
        public void IListTManyElementsLastIsNotDefault()
        {
            int?[] source = [-10, 2, 4, 3, 0, 2, null, 19];
            int? expected = 19;

            Assert.IsType<IList<int?>>(source, exactMatch: false);

            Assert.Equal(expected, source.LastOrDefault());
        }

        [Fact]
        public void IListTManyElementsLastHasDefault()
        {
            int?[] source = [-10, 2, 4, 3, 0, 2, null];
            int? expected = null;

            Assert.IsType<IList<int?>>(source, exactMatch: false);

            Assert.Equal(expected, source.LastOrDefault(5));
        }

        [Fact]
        public void IListTManyElementsLastIsHasDefault()
        {
            int?[] source = [-10, 2, 4, 3, 0, 2, null, 19];
            int? expected = 19;

            Assert.IsType<IList<int?>>(source, exactMatch: false);

            Assert.Equal(expected, source.LastOrDefault(5));
        }

        [Fact]
        public void EmptySourcePredicate()
        {
            int?[] source = [];

            Assert.All(CreateSources(source), source =>
            {
                Assert.Null(source.LastOrDefault(x => true));
                Assert.Null(source.LastOrDefault(x => false));
            });
        }

        [Fact]
        public void OneElementTruePredicate()
        {
            int[] source = [4];
            Func<int, bool> predicate = IsEven;
            int expected = 4;

            Assert.All(CreateSources(source), source => Assert.Equal(expected, source.LastOrDefault(predicate)));
        }

        [Fact]
        public void OneElementTruePredicateDefault()
        {
            int[] source = [4];
            Func<int, bool> predicate = IsEven;
            int expected = 4;

            Assert.All(CreateSources(source), source => Assert.Equal(expected, source.LastOrDefault(predicate, 5)));
        }

        [Fact]
        public void ManyElementsPredicateFalseForAll()
        {
            int[] source = [9, 5, 1, 3, 17, 21];
            Func<int, bool> predicate = IsEven;
            int expected = default;

            Assert.All(CreateSources(source), source => Assert.Equal(expected, source.LastOrDefault(predicate)));
        }

        [Fact]
        public void ManyElementsPredicateFalseForAllDefault()
        {
            int[] source = [9, 5, 1, 3, 17, 21];
            Func<int, bool> predicate = IsEven;
            int expected = 5;

            Assert.All(CreateSources(source), source => Assert.Equal(expected, source.LastOrDefault(predicate, 5)));
        }

        [Fact]
        public void PredicateTrueOnlyForLast()
        {
            int[] source = [9, 5, 1, 3, 17, 21, 50];
            Func<int, bool> predicate = IsEven;
            int expected = 50;

            Assert.All(CreateSources(source), source => Assert.Equal(expected, source.LastOrDefault(predicate)));
        }

        [Fact]
        public void PredicateTrueForSome()
        {
            int[] source = [3, 7, 10, 7, 9, 2, 11, 18, 13, 9];
            Func<int, bool> predicate = IsEven;
            int expected = 18;

            Assert.All(CreateSources(source), source => Assert.Equal(expected, source.LastOrDefault(predicate)));
        }

        [Fact]
        public void PredicateTrueForSomeRunOnce()
        {
            int[] source = [3, 7, 10, 7, 9, 2, 11, 18, 13, 9];
            Func<int, bool> predicate = IsEven;
            int expected = 18;

            Assert.All(CreateSources(source), source => Assert.Equal(expected, source.LastOrDefault(predicate)));
        }

        [Fact]
        public void NullSource()
        {
            AssertExtensions.Throws<ArgumentNullException>("source", () => ((IReadOnlyList<int>)null).LastOrDefault());
        }

        [Fact]
        public void NullSourcePredicateUsed()
        {
            AssertExtensions.Throws<ArgumentNullException>("source", () => ((IReadOnlyList<int>)null).LastOrDefault(i => i != 2));
        }

        [Fact]
        public void NullPredicate()
        {
            Func<int, bool> predicate = null;
            AssertExtensions.Throws<ArgumentNullException>("predicate", () => Indexerable.Range(0, 3).LastOrDefault(predicate));
        }
    }
}
