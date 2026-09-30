using System;
using System.Collections.Generic;
using Xunit;

namespace Indexer.Linq.Tests
{
    public class RangeTests : RangeTestsBase
    {
        protected override IReadOnlyList<int> GetRange(int start, int count) => Indexerable.Range(start, count);

        [Fact]
        public void Range_ThrowExceptionOnNegativeCount()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => GetRange(1, -1));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => GetRange(1, int.MinValue));
        }

        [Fact]
        public void Range_ThrowExceptionOnOverflow()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => GetRange(1000, int.MaxValue));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => GetRange(int.MaxValue, 1000));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => GetRange(int.MaxValue - 10, 20));
        }
    }

    public abstract class RangeTestsBase : IndexerableTests
    {
        protected abstract IReadOnlyList<int> GetRange(int start, int count);

        [Fact]
        public void Range_ProduceCorrectSequence()
        {
            IReadOnlyList<int> rangeSequence = GetRange(1, 100);
            int expected = 0;
            foreach (int val in rangeSequence)
            {
                expected++;
                Assert.Equal(expected, val);
            }

            Assert.Equal(100, expected);
        }

        public static IEnumerable<TheoryDataRow<int>> Range_ToArray_ProduceCorrectResult_MemberData()
        {
            for (int i = 0; i < 64; i++)
            {
                yield return new TheoryDataRow<int>(i);
            }
        }

        [Theory]
        [MemberData(nameof(Range_ToArray_ProduceCorrectResult_MemberData))]
        public void Range_ToArray_ProduceCorrectResult(int length)
        {
            IReadOnlyList<int> array = GetRange(1, length);
            Assert.Equal(length, array.Count);
            for (int i = 0; i < array.Count; i++)
            {
                Assert.Equal(i + 1, array[i]);
            }
        }

        [Fact]
        public void Range_ToList_ProduceCorrectResult()
        {
            IReadOnlyList<int> list = GetRange(1, 100);
            Assert.Equal(100, list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                Assert.Equal(i + 1, list[i]);
            }
        }

        [Fact]
        public void Range_ZeroCountLeadToEmptySequence()
        {
            IReadOnlyList<int> array = GetRange(1, 0);
            IReadOnlyList<int> array2 = GetRange(int.MinValue, 0);
            IReadOnlyList<int> array3 = GetRange(int.MaxValue, 0);
            Assert.Empty(array);
            Assert.Empty(array2);
            Assert.Empty(array3);
        }

        [Fact]
        public void Range_NotEnumerateAfterEnd()
        {
            using IEnumerator<int> rangeEnum = GetRange(1, 1).GetEnumerator();
            Assert.True(rangeEnum.MoveNext());
            Assert.False(rangeEnum.MoveNext());
            Assert.False(rangeEnum.MoveNext());
        }

        [Fact]
        public void Range_GetEnumeratorReturnUniqueInstances()
        {
            IReadOnlyList<int> rangeEnumerable = GetRange(1, 1);
            using IEnumerator<int> enum1 = rangeEnumerable.GetEnumerator();
            using IEnumerator<int> enum2 = rangeEnumerable.GetEnumerator();
            Assert.NotSame(enum1, enum2);
        }

        [Fact]
        public void Range_ToInt32MaxValue()
        {
            int from = int.MaxValue - 3;
            int count = 4;
            IReadOnlyList<int> rangeEnumerable = GetRange(from, count);

            Assert.Equal(count, rangeEnumerable.Count);

            int[] expected = [int.MaxValue - 3, int.MaxValue - 2, int.MaxValue - 1, int.MaxValue];
            Assert.Equal(expected, rangeEnumerable);
        }

        [Fact]
        public void RepeatedCallsSameResults()
        {
            Assert.Equal(GetRange(-1, 2), GetRange(-1, 2));
            Assert.Equal(GetRange(0, 0), GetRange(0, 0));
        }

        [Fact]
        public void NegativeStart()
        {
            int start = -5;
            int count = 1;
            int[] expected = [-5];

            Assert.Equal(expected, GetRange(start, count));
        }

        [Fact]
        public void ArbitraryStart()
        {
            int start = 12;
            int count = 6;
            int[] expected = [12, 13, 14, 15, 16, 17];

            Assert.Equal(expected, GetRange(start, count));
        }

        [Fact]
        public void Take()
        {
            Assert.Equal(GetRange(0, 10), GetRange(0, 20).Take(10));
        }

        [Fact]
        public void TakeExcessive()
        {
            Assert.Equal(GetRange(0, 10), GetRange(0, 10).Take(int.MaxValue));
        }

        [Fact]
        public void Skip()
        {
            Assert.Equal(GetRange(10, 10), GetRange(0, 20).Skip(10));
        }

        [Fact]
        public void SkipExcessive()
        {
            Assert.Empty(GetRange(10, 10).Skip(20));
        }

        [Fact]
        public void SkipTakeCanOnlyBeOne()
        {
            Assert.Equal([1], GetRange(1, 10).Take(1));
            Assert.Equal([2], GetRange(1, 10).Skip(1).Take(1));
            Assert.Equal([3], GetRange(1, 10).Take(3).Skip(2));
            Assert.Equal([1], GetRange(1, 10).Take(3).Take(1));
        }

        [Fact]
        public void ElementAt()
        {
            Assert.Equal(4, GetRange(0, 10)[4]);
        }

        [Fact]
        public void ElementAtExcessiveThrows()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => _ = GetRange(0, 10)[100]);
        }

        [Fact]
        public void First()
        {
            Assert.Equal(57, GetRange(57, 1000000000)[0]);
        }

        [Fact]
        public void IListImplementationIsValid()
        {
            Validate(GetRange(42, 10), [42, 43, 44, 45, 46, 47, 48, 49, 50, 51]);
            Validate(GetRange(42, 10).Skip(3).Take(4), [45, 46, 47, 48]);

            static void Validate(IReadOnlyList<int> e, int[] expected)
            {
                IReadOnlyList<int> roList = Assert.IsType<IReadOnlyList<int>>(e, exactMatch: false);

                AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => _ = roList[-1]);
                AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => _ = roList[expected.Length]);

                Assert.Equal(expected.Length, roList.Count);

                Assert.DoesNotContain(expected[0] - 1, roList);
                Assert.DoesNotContain(expected[^1] + 1, roList);
                Assert.Equal(-1, roList.IndexOf(expected[0] - 1));
                Assert.Equal(-1, roList.IndexOf(expected[^1] + 1));
                Assert.All(expected, i => Assert.Contains(i, roList));
                Assert.All(expected, i => Assert.Equal(Array.IndexOf(expected, i), roList.IndexOf(i)));
                for (int i = 0; i < expected.Length; i++)
                {
                    Assert.Equal(expected[i], roList[i]);
                }
            }
        }
    }
}
