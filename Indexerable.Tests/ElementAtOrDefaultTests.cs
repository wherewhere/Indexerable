using System;
using System.Collections.Generic;
using Xunit;

namespace Indexer.Linq.Tests
{
    public class ElementAtOrDefaultTests : IndexerableTests
    {
        [Fact]
        public void SameResultsRepeatCallsIntQuery()
        {
            IReadOnlyList<int> q = from x in new[] { 9999, 0, 888, -1, 66, -777, 1, 2, -12345 }
                                   select x;
            Assert.Equal(q.ElementAtOrDefault(3), q.ElementAtOrDefault(3));
            Assert.Equal(q.ElementAtOrDefault(new Index(3)), q.ElementAtOrDefault(new Index(3)));
            Assert.Equal(q.ElementAtOrDefault(^6), q.ElementAtOrDefault(^6));
        }

        [Fact]
        public void SameResultsRepeatCallsStringQuery()
        {
            IReadOnlyList<string> q = from x in new[] { "!@#$%^", "C", "AAA", "", "Calling Twice", "SoS", string.Empty }
                                      select x;
            Assert.Equal(q.ElementAtOrDefault(4), q.ElementAtOrDefault(4));
            Assert.Equal(q.ElementAtOrDefault(new Index(4)), q.ElementAtOrDefault(new Index(4)));
            Assert.Equal(q.ElementAtOrDefault(^2), q.ElementAtOrDefault(^2));
        }

        public static IEnumerable<TheoryDataRow<IReadOnlyList<int>, int, int, int>> TestData()
        {
            yield return new(Indexerable.Range(9, 1), 0, 1, 9);
            yield return new(Indexerable.Range(9, 10), 9, 1, 18);
            yield return new(Indexerable.Range(-4, 10), 3, 7, -1);

            yield return new((int[])[1, 2, 3, 4], 4, 0, 0);
            yield return new((int[])[], 0, 0, 0);
            yield return new((int[])[-4], 0, 1, -4);
            yield return new((int[])[9, 8, 0, -5, 10], 4, 1, 10);

            yield return new(Indexerable.Range(-4, 5), -1, 6, 0);
            yield return new(Indexerable.Range(5, 5), 5, 0, 0);
            yield return new(Indexerable.Range(0, 0), 0, 0, 0);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void ElementAtOrDefault(IReadOnlyList<int> source, int index, int indexFromEnd, int expected)
        {
            Assert.Equal(expected, source.ElementAtOrDefault(index));

            if (index >= 0)
            {
                Assert.Equal(expected, source.ElementAtOrDefault(new Index(index)));
            }

            Assert.Equal(expected, source.ElementAtOrDefault(^indexFromEnd));
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void ElementAtOrDefaultRunOnce(IReadOnlyList<int> source, int index, int indexFromEnd, int expected)
        {
            Assert.Equal(expected, source.ElementAtOrDefault(index));

            if (index >= 0)
            {
                Assert.Equal(expected, source.ElementAtOrDefault(new Index(index)));
            }

            Assert.Equal(expected, source.ElementAtOrDefault(^indexFromEnd));
        }

        [Fact]
        public void NullableArray_InvalidIndex_ReturnsNull()
        {
            int?[] source = [9, 8];
            Assert.Null(source.ElementAtOrDefault(-1));
            Assert.Null(source.ElementAtOrDefault(3));
            Assert.Null(source.ElementAtOrDefault(int.MaxValue));
            Assert.Null(source.ElementAtOrDefault(int.MinValue));

            Assert.Null(source.ElementAtOrDefault(^3));
            Assert.Null(source.ElementAtOrDefault(new Index(3)));
            Assert.Null(source.ElementAtOrDefault(new Index(int.MaxValue)));
            Assert.Null(source.ElementAtOrDefault(^int.MaxValue));
        }

        [Fact]
        public void NullableArray_ValidIndex_ReturnsCorrectObject()
        {
            int?[] source = [9, 8, null, -5, 10];

            Assert.Null(source.ElementAtOrDefault(2));
            Assert.Equal(-5, source.ElementAtOrDefault(3));

            Assert.Null(source.ElementAtOrDefault(new Index(2)));
            Assert.Equal(-5, source.ElementAtOrDefault(new Index(3)));

            Assert.Null(source.ElementAtOrDefault(^3));
            Assert.Equal(-5, source.ElementAtOrDefault(^2));
        }

        [Fact]
        public void NullSource_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("source", () => ((IReadOnlyList<int>)null).ElementAtOrDefault(2));
            Assert.Throws<ArgumentNullException>("source", () => ((IReadOnlyList<int>)null).ElementAtOrDefault(new Index(2)));
            Assert.Throws<ArgumentNullException>("source", () => ((IReadOnlyList<int>)null).ElementAtOrDefault(^2));
        }

        [Fact]
        public void MutableSource()
        {
            List<int> source = [0, 1, 2, 3, 4];
            Assert.Equal(2, source.ElementAtOrDefault(2));
            Assert.Equal(2, source.ElementAtOrDefault(new Index(2)));
            Assert.Equal(2, source.ElementAtOrDefault(^3));

            source.InsertRange(3, [-1, -2]);
            source.RemoveAt(0);
            Assert.Equal(-1, source.ElementAtOrDefault(2));
            Assert.Equal(-1, source.ElementAtOrDefault(new Index(2)));
            Assert.Equal(-1, source.ElementAtOrDefault(^4));
        }

        [Fact]
        public void NonEmptySource_Consistency()
        {
            int?[] source = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];

            Assert.Equal(5, source.ElementAtOrDefault(5));
            Assert.Equal(5, source.ElementAtOrDefault(new Index(5)));
            Assert.Equal(5, source.ElementAtOrDefault(^5));

            Assert.Equal(0, source.ElementAtOrDefault(0));
            Assert.Equal(0, source.ElementAtOrDefault(new Index(0)));
            Assert.Equal(0, source.ElementAtOrDefault(^10));

            Assert.Equal(9, source.ElementAtOrDefault(9));
            Assert.Equal(9, source.ElementAtOrDefault(new Index(9)));
            Assert.Equal(9, source.ElementAtOrDefault(^1));

            Assert.Null(source.ElementAtOrDefault(-1));
            Assert.Null(source.ElementAtOrDefault(^11));

            Assert.Null(source.ElementAtOrDefault(10));
            Assert.Null(source.ElementAtOrDefault(new Index(10)));
            Assert.Null(source.ElementAtOrDefault(^0));

            Assert.Null(source.ElementAtOrDefault(int.MinValue));
            Assert.Null(source.ElementAtOrDefault(^int.MaxValue));

            Assert.Null(source.ElementAtOrDefault(int.MaxValue));
            Assert.Null(source.ElementAtOrDefault(new Index(int.MaxValue)));
        }

        [Fact]
        public void EmptySource_Consistency()
        {
            int?[] source = [];

            Assert.Null(source.ElementAtOrDefault(1));
            Assert.Null(source.ElementAtOrDefault(-1));
            Assert.Null(source.ElementAtOrDefault(new Index(1)));
            Assert.Null(source.ElementAtOrDefault(^1));

            Assert.Null(source.ElementAtOrDefault(0));
            Assert.Null(source.ElementAtOrDefault(new Index(0)));
            Assert.Null(source.ElementAtOrDefault(^0));

            Assert.Null(source.ElementAtOrDefault(int.MinValue));
            Assert.Null(source.ElementAtOrDefault(^int.MaxValue));

            Assert.Null(source.ElementAtOrDefault(int.MaxValue));
            Assert.Null(source.ElementAtOrDefault(new Index(int.MaxValue)));
        }
    }
}
