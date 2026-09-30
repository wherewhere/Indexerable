using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Indexer.Linq.Tests
{
    public class TakeTests : IndexerableTests
    {
        [Fact]
        public void SameResultsRepeatCallsIntQuery()
        {
            IReadOnlyList<int> q = from x in new[] { 9999, 0, 888, -1, 66, -777, 1, 2, -12345 }
                                   select x;

            Assert.Equal(q.Take(9), q.Take(9));

            Assert.Equal(q.Take(0..9), q.Take(0..9));
            Assert.Equal(q.Take(^9..9), q.Take(^9..9));
            Assert.Equal(q.Take(0..^0), q.Take(0..^0));
            Assert.Equal(q.Take(^9..^0), q.Take(^9..^0));
        }

        [Fact]
        public void SameResultsRepeatCallsStringQuery()
        {
            IReadOnlyList<string> q = from x in new[] { "!@#$%^", "C", "AAA", "", "Calling Twice", "SoS", string.Empty }
                                      select x;

            Assert.Equal(q.Take(7), q.Take(7));

            Assert.Equal(q.Take(0..7), q.Take(0..7));
            Assert.Equal(q.Take(^7..7), q.Take(^7..7));
            Assert.Equal(q.Take(0..^0), q.Take(0..^0));
            Assert.Equal(q.Take(^7..^0), q.Take(^7..^0));
        }

        [Fact]
        public void SourceEmptyCountPositive()
        {
            foreach (IReadOnlyList<int> source in CreateSources<int>([]))
            {
                Assert.Empty(source.Take(5));

                Assert.Empty(source.Take(0..5));
                Assert.Empty(source.Take(^5..5));
                Assert.Empty(source.Take(0..^0));
                Assert.Empty(source.Take(^5..^0));
            }
        }

        [Fact]
        public void SourceNonEmptyCountNegative()
        {
            foreach (IReadOnlyList<int> source in CreateSources([2, 5, 9, 1]))
            {
                Assert.Empty(source.Take(-5));
                Assert.Empty(source.Take(^9..0));
            }
        }

        [Fact]
        public void SourceNonEmptyCountZero()
        {
            foreach (IReadOnlyList<int> source in CreateSources([2, 5, 9, 1]))
            {
                Assert.Empty(source.Take(0));

                Assert.Empty(source.Take(0..0));
                Assert.Empty(source.Take(^4..0));
                Assert.Empty(source.Take(0..^4));
                Assert.Empty(source.Take(^4..^4));
            }
        }

        [Fact]
        public void SourceNonEmptyCountOne()
        {
            int[] expected = [2];

            foreach (IReadOnlyList<int> source in CreateSources([2, 5, 9, 1]))
            {
                Assert.Equal(expected, source.Take(1));

                Assert.Equal(expected, source.Take(0..1));
                Assert.Equal(expected, source.Take(^4..1));
                Assert.Equal(expected, source.Take(0..^3));
                Assert.Equal(expected, source.Take(^4..^3));
            }
        }

        [Fact]
        public void SourceNonEmptyTakeAllExactly()
        {
            foreach (IReadOnlyList<int> source in CreateSources([2, 5, 9, 1]))
            {
                Assert.Equal(source, source.Take(4));

                Assert.Equal(source, source.Take(0..4));
                Assert.Equal(source, source.Take(^4..4));
                Assert.Equal(source, source.Take(0..^0));
                Assert.Equal(source, source.Take(^4..^0));
            }
        }

        [Fact]
        public void SourceNonEmptyTakeAllButOne()
        {
            int[] expected = [2, 5, 9];

            foreach (IReadOnlyList<int> source in CreateSources([2, 5, 9, 1]))
            {
                Assert.Equal(expected, source.Take(3));

                Assert.Equal(expected, source.Take(0..3));
                Assert.Equal(expected, source.Take(^4..3));
                Assert.Equal(expected, source.Take(0..^1));
                Assert.Equal(expected, source.Take(^4..^1));
            }
        }

        [Fact]
        public void SourceNonEmptyTakeExcessive()
        {
            foreach (IReadOnlyList<int?> source in CreateSources<int?>([2, 5, null, 9, 1]))
            {
                Assert.Equal(source, source.Take(5));

                Assert.Equal(source, source.Take(0..5));
                Assert.Equal(source, source.Take(^6..6));
            }
        }

        [Fact]
        public void ThrowsOnNullSource()
        {
            int[] source = null;
            Assert.Throws<ArgumentNullException>("source", () => source.Take(5));

            Assert.Throws<ArgumentNullException>("source", () => source.Take(0..5));
            Assert.Throws<ArgumentNullException>("source", () => source.Take(^5..5));
            Assert.Throws<ArgumentNullException>("source", () => source.Take(0..^0));
            Assert.Throws<ArgumentNullException>("source", () => source.Take(^5..^0));
        }

        [Fact]
        public void Count()
        {
            Assert.Equal(2, Indexerable.Range(0, 3).Take(2).Count);
            Assert.Equal(2, ((int[])[1, 2, 3]).Take(2).Count);
            Assert.Empty(Indexerable.Range(0, 3).Take(0));

            Assert.Equal(2, Indexerable.Range(0, 3).Take(0..2).Count);
            Assert.Equal(2, ((int[])[1, 2, 3]).Take(0..2).Count);
            Assert.Empty(Indexerable.Range(0, 3).Take(0..0));

            Assert.Equal(2, Indexerable.Range(0, 3).Take(^3..2).Count);
            Assert.Equal(2, ((int[])[1, 2, 3]).Take(^3..2).Count);
            Assert.Empty(Indexerable.Range(0, 3).Take(^3..0));

            Assert.Equal(2, Indexerable.Range(0, 3).Take(0..^1).Count);
            Assert.Equal(2, ((int[])[1, 2, 3]).Take(0..^1).Count);
            Assert.Empty(Indexerable.Range(0, 3).Take(0..^3));

            Assert.Equal(2, Indexerable.Range(0, 3).Take(^3..^1).Count);
            Assert.Equal(2, ((int[])[1, 2, 3]).Take(^3..^1).Count);
            Assert.Empty(Indexerable.Range(0, 3).Take(^3..^3));
        }

        [Fact]
        public void FollowWithTake()
        {
            int[] expected = [5, 6];

            foreach (IReadOnlyList<int> source in CreateSources([5, 6, 7, 8]))
            {
                Assert.Equal(expected, source.Take(5).Take(3).Take(2).Take(40));

                Assert.Equal(expected, source.Take(0..5).Take(0..3).Take(0..2).Take(0..40));
                Assert.Equal(expected, source.Take(^4..5).Take(^4..3).Take(^3..2).Take(^2..40));
                Assert.Equal(expected, source.Take(0..^0).Take(0..^1).Take(0..^1).Take(0..^0));
                Assert.Equal(expected, source.Take(^4..^0).Take(^4..^1).Take(^3..^1).Take(^2..^0));
            }
        }

        [Fact]
        public void FollowWithSkip()
        {
            int[] expected = [3, 4, 5];

            foreach (IReadOnlyList<int> source in CreateSources([1, 2, 3, 4, 5, 6]))
            {
                Assert.Equal(expected, source.Take(5).Skip(2).Skip(-4));

                Assert.Equal(expected, source.Take(0..5).Skip(2).Skip(-4));
                Assert.Equal(expected, source.Take(^6..5).Skip(2).Skip(-4));
                Assert.Equal(expected, source.Take(0..^1).Skip(2).Skip(-4));
                Assert.Equal(expected, source.Take(^6..^1).Skip(2).Skip(-4));
            }
        }

        [Fact]
        public void ElementAt()
        {
            foreach (IReadOnlyList<int> source in CreateSources([1, 2, 3, 4, 5, 6]))
            {
                IReadOnlyList<int> taken0 = source.Take(3);
                Assert.Equal(1, taken0[0]);
                Assert.Equal(3, taken0[2]);
                Assert.Throws<ArgumentOutOfRangeException>("index", () => taken0[-1]);
                Assert.Throws<ArgumentOutOfRangeException>("index", () => taken0[3]);

                IReadOnlyList<int> taken1 = source.Take(0..3);
                Assert.Equal(1, taken1[0]);
                Assert.Equal(3, taken1[2]);
                Assert.Throws<ArgumentOutOfRangeException>("index", () => taken1[-1]);
                Assert.Throws<ArgumentOutOfRangeException>("index", () => taken1[3]);

                IReadOnlyList<int> taken2 = source.Take(^6..3);
                Assert.Equal(1, taken2[0]);
                Assert.Equal(3, taken2[2]);
                Assert.Throws<ArgumentOutOfRangeException>("index", () => taken2[-1]);
                Assert.Throws<ArgumentOutOfRangeException>("index", () => taken2[3]);

                IReadOnlyList<int> taken3 = source.Take(0..^3);
                Assert.Equal(1, taken3[0]);
                Assert.Equal(3, taken3[2]);
                Assert.Throws<ArgumentOutOfRangeException>("index", () => taken3[-1]);
                Assert.Throws<ArgumentOutOfRangeException>("index", () => taken3[3]);

                IReadOnlyList<int> taken4 = source.Take(^6..^3);
                Assert.Equal(1, taken4[0]);
                Assert.Equal(3, taken4[2]);
                Assert.Throws<ArgumentOutOfRangeException>("index", () => taken4[-1]);
                Assert.Throws<ArgumentOutOfRangeException>("index", () => taken4[3]);
            }
        }

        [Fact]
        public void First()
        {
            foreach (IReadOnlyList<int> source in CreateSources([1, 2, 3, 4, 5]))
            {
                Assert.Equal(1, source.Take(1)[0]);
                Assert.Equal(1, source.Take(4)[0]);
                Assert.Equal(1, source.Take(40)[0]);
                Assert.Throws<ArgumentOutOfRangeException>(() => source.Take(0)[0]);
                Assert.Throws<ArgumentOutOfRangeException>(() => source.Skip(5).Take(10)[0]);

                Assert.Equal(1, source.Take(0..1)[0]);
                Assert.Equal(1, source.Take(0..4)[0]);
                Assert.Equal(1, source.Take(0..40)[0]);
                Assert.Throws<ArgumentOutOfRangeException>(() => source.Take(0..0)[0]);
                Assert.Throws<ArgumentOutOfRangeException>(() => source.Skip(5).Take(0..10)[0]);

                Assert.Equal(1, source.Take(^5..1)[0]);
                Assert.Equal(1, source.Take(^5..4)[0]);
                Assert.Equal(1, source.Take(^5..40)[0]);
                Assert.Throws<ArgumentOutOfRangeException>(() => source.Take(^5..0)[0]);
                Assert.Throws<ArgumentOutOfRangeException>(() => source.Skip(5).Take(^5..10)[0]);

                Assert.Equal(1, source.Take(0..^4)[0]);
                Assert.Equal(1, source.Take(0..^1)[0]);
                Assert.Equal(1, source.Take(0..^0)[0]);
                Assert.Throws<ArgumentOutOfRangeException>(() => source.Take(0..^5)[0]);
                Assert.Throws<ArgumentOutOfRangeException>(() => source.Skip(5).Take(0..^5)[0]);

                Assert.Equal(1, source.Take(^5..^4)[0]);
                Assert.Equal(1, source.Take(^5..^1)[0]);
                Assert.Equal(1, source.Take(^5..^0)[0]);
                Assert.Throws<ArgumentOutOfRangeException>(() => source.Take(^5..^5)[0]);
                Assert.Throws<ArgumentOutOfRangeException>(() => source.Skip(5).Take(^10..^0)[0]);
            }
        }

        [Fact]
        public void Last()
        {
            foreach (IReadOnlyList<int> source in CreateSources([1, 2, 3, 4, 5]))
            {
                Assert.Equal(1, source.Take(1)[^1]);
                Assert.Equal(2, source.Take(2)[^1]);
                Assert.Equal(3, source.Take(3)[^1]);
                Assert.Equal(4, source.Take(4)[^1]);
                Assert.Equal(5, source.Take(5)[^1]);
                Assert.Equal(5, source.Take(6)[^1]);
                Assert.Equal(5, source.Take(40)[^1]);
                Assert.Throws<ArgumentOutOfRangeException>(() => source.Take(0)[^1]);
                Assert.Throws<ArgumentOutOfRangeException>(() => Array.Empty<int>().Take(40)[^1]);

                Assert.Equal(1, source.Take(0..1)[^1]);
                Assert.Equal(5, source.Take(0..5)[^1]);
                Assert.Equal(5, source.Take(0..40)[^1]);
                Assert.Throws<ArgumentOutOfRangeException>(() => source.Take(0..0)[^1]);
                Assert.Throws<ArgumentOutOfRangeException>(() => Array.Empty<int>().Take(0..40)[^1]);

                Assert.Equal(1, source.Take(^5..1)[^1]);
                Assert.Equal(5, source.Take(^5..5)[^1]);
                Assert.Equal(5, source.Take(^5..40)[^1]);
                Assert.Throws<ArgumentOutOfRangeException>(() => source.Take(^5..0)[^1]);
                Assert.Throws<ArgumentOutOfRangeException>(() => Array.Empty<int>().Take(^5..40)[^1]);

                Assert.Equal(1, source.Take(0..^4)[^1]);
                Assert.Equal(5, source.Take(0..^0)[^1]);
                Assert.Equal(5, source.Take(3..^0)[^1]);
                Assert.Throws<ArgumentOutOfRangeException>(() => source.Take(0..^5)[^1]);
                Assert.Throws<ArgumentOutOfRangeException>(() => Array.Empty<int>().Take(0..^0)[^1]);

                Assert.Equal(1, source.Take(^5..^4)[^1]);
                Assert.Equal(5, source.Take(^5..^0)[^1]);
                Assert.Equal(5, source.Take(^5..^0)[^1]);
                Assert.Throws<ArgumentOutOfRangeException>(() => source.Take(^5..^5)[^1]);
                Assert.Throws<ArgumentOutOfRangeException>(() => Array.Empty<int>().Take(^40..^0)[^1]);
            }
        }

        [Fact]
        public void ToArray()
        {
            foreach (IReadOnlyList<int> source in CreateSources([1, 2, 3, 4, 5]))
            {
                Assert.Equal([1, 2, 3, 4, 5], (int[])[.. source.Take(5)]);
                Assert.Equal([1, 2, 3, 4, 5], (int[])[.. source.Take(6)]);
                Assert.Equal([1, 2, 3, 4, 5], (int[])[.. source.Take(40)]);
                Assert.Equal([1, 2, 3, 4], (int[])[.. source.Take(4)]);
                Assert.Equal(1, source.Take(1).ToArray().Single());
                Assert.Empty(source.Take(0).ToArray());
                Assert.Empty(source.Take(-10).ToArray());

                Assert.Equal([1, 2, 3, 4, 5], (int[])[.. source.Take(0..5)]);
                Assert.Equal([1, 2, 3, 4, 5], (int[])[.. source.Take(0..6)]);
                Assert.Equal([1, 2, 3, 4, 5], (int[])[.. source.Take(0..40)]);
                Assert.Equal([1, 2, 3, 4], (int[])[.. source.Take(0..4)]);
                Assert.Equal(1, source.Take(0..1).ToArray().Single());
                Assert.Empty(source.Take(0..0).ToArray());

                Assert.Equal([1, 2, 3, 4, 5], (int[])[.. source.Take(^5..5)]);
                Assert.Equal([1, 2, 3, 4, 5], (int[])[.. source.Take(^5..6)]);
                Assert.Equal([1, 2, 3, 4, 5], (int[])[.. source.Take(^5..40)]);
                Assert.Equal([1, 2, 3, 4], (int[])[.. source.Take(^5..4)]);
                Assert.Equal(1, source.Take(^5..1).ToArray().Single());
                Assert.Empty(source.Take(^5..0).ToArray());
                Assert.Empty(source.Take(^15..0).ToArray());

                Assert.Equal([1, 2, 3, 4, 5], (int[])[.. source.Take(0..^0)]);
                Assert.Equal([1, 2, 3, 4], (int[])[.. source.Take(0..^1)]);
                Assert.Equal(1, source.Take(0..^4).ToArray().Single());
                Assert.Empty(source.Take(0..^5).ToArray());
                Assert.Empty(source.Take(0..^15).ToArray());

                Assert.Equal([1, 2, 3, 4, 5], (int[])[.. source.Take(^5..^0)]);
                Assert.Equal([1, 2, 3, 4, 5], (int[])[.. source.Take(^6..^0)]);
                Assert.Equal([1, 2, 3, 4, 5], (int[])[.. source.Take(^45..^0)]);
                Assert.Equal([1, 2, 3, 4], (int[])[.. source.Take(^5..^1)]);
                Assert.Equal(1, source.Take(^5..^4).ToArray().Single());
                Assert.Empty(source.Take(^5..^5).ToArray());
                Assert.Empty(source.Take(^15..^5).ToArray());
            }
        }

        [Fact]
        public void ToList()
        {
            foreach (IReadOnlyList<int> source in CreateSources([1, 2, 3, 4, 5]))
            {
                Assert.Equal([1, 2, 3, 4, 5], source.Take(5).ToList());
                Assert.Equal([1, 2, 3, 4, 5], source.Take(6).ToList());
                Assert.Equal([1, 2, 3, 4, 5], source.Take(40).ToList());
                Assert.Equal([1, 2, 3, 4], source.Take(4).ToList());
                Assert.Equal(1, source.Take(1).ToList().Single());
                Assert.Empty(source.Take(0).ToList());
                Assert.Empty(source.Take(-10).ToList());

                Assert.Equal([1, 2, 3, 4, 5], source.Take(0..5).ToList());
                Assert.Equal([1, 2, 3, 4, 5], source.Take(0..6).ToList());
                Assert.Equal([1, 2, 3, 4, 5], source.Take(0..40).ToList());
                Assert.Equal([1, 2, 3, 4], source.Take(0..4).ToList());
                Assert.Equal(1, source.Take(0..1).ToList().Single());
                Assert.Empty(source.Take(0..0).ToList());

                Assert.Equal([1, 2, 3, 4, 5], source.Take(^5..5).ToList());
                Assert.Equal([1, 2, 3, 4, 5], source.Take(^5..6).ToList());
                Assert.Equal([1, 2, 3, 4, 5], source.Take(^5..40).ToList());
                Assert.Equal([1, 2, 3, 4], source.Take(^5..4).ToList());
                Assert.Equal(1, source.Take(^5..1).ToList().Single());
                Assert.Empty(source.Take(^5..0).ToList());
                Assert.Empty(source.Take(^15..0).ToList());

                Assert.Equal([1, 2, 3, 4, 5], source.Take(0..^0).ToList());
                Assert.Equal([1, 2, 3, 4], source.Take(0..^1).ToList());
                Assert.Equal(1, source.Take(0..^4).ToList().Single());
                Assert.Empty(source.Take(0..^5).ToList());
                Assert.Empty(source.Take(0..^15).ToList());

                Assert.Equal([1, 2, 3, 4, 5], source.Take(^5..^0).ToList());
                Assert.Equal([1, 2, 3, 4, 5], source.Take(^6..^0).ToList());
                Assert.Equal([1, 2, 3, 4, 5], source.Take(^45..^0).ToList());
                Assert.Equal([1, 2, 3, 4], source.Take(^5..^1).ToList());
                Assert.Equal(1, source.Take(^5..^4).ToList().Single());
                Assert.Empty(source.Take(^5..^5).ToList());
                Assert.Empty(source.Take(^15..^5).ToList());
            }
        }

        [Fact]
        public void TakeCanOnlyBeOneList()
        {
            foreach (IReadOnlyList<int> source in CreateSources([2, 4, 6, 8, 10]))
            {
                Assert.Equal([2], source.Take(1));
                Assert.Equal([4], source.Skip(1).Take(1));
                Assert.Equal([6], source.Take(3).Skip(2));
                Assert.Equal([2], source.Take(3).Take(1));

                Assert.Equal([2], source.Take(0..1));
                Assert.Equal([4], source.Skip(1).Take(0..1));
                Assert.Equal([6], source.Take(0..3).Skip(2));
                Assert.Equal([2], source.Take(0..3).Take(0..1));

                Assert.Equal([2], source.Take(^5..1));
                Assert.Equal([4], source.Skip(1).Take(^4..1));
                Assert.Equal([6], source.Take(^5..3).Skip(2));
                Assert.Equal([2], source.Take(^5..3).Take(^4..1));

                Assert.Equal([2], source.Take(0..^4));
                Assert.Equal([4], source.Skip(1).Take(0..^3));
                Assert.Equal([6], source.Take(0..^2).Skip(2));
                Assert.Equal([2], source.Take(0..^2).Take(0..^2));

                Assert.Equal([2], source.Take(^5..^4));
                Assert.Equal([4], source.Skip(1).Take(^4..^3));
                Assert.Equal([6], source.Take(^5..^2).Skip(2));
                Assert.Equal([2], source.Take(^5..^2).Take(^4..^2));
            }
        }

        [Fact]
        public void RepeatEnumerating()
        {
            foreach (IReadOnlyList<int> source in CreateSources([1, 2, 3, 4, 5]))
            {
                IReadOnlyList<int> taken1 = source.Take(3);
                Assert.Equal(taken1, taken1);

                IReadOnlyList<int> taken2 = source.Take(0..3);
                Assert.Equal(taken2, taken2);

                IReadOnlyList<int> taken3 = source.Take(^5..3);
                Assert.Equal(taken3, taken3);

                IReadOnlyList<int> taken4 = source.Take(0..^2);
                Assert.Equal(taken4, taken4);

                IReadOnlyList<int> taken5 = source.Take(^5..^2);
                Assert.Equal(taken5, taken5);
            }
        }

        [Fact]
        public void LazyOverflowRegression()
        {
            IReadOnlyList<int> range = Indexerable.Range(1, 100);
            IReadOnlyList<int> skipped = range.Skip(42); // Min index is 42.
            IReadOnlyList<int> taken1 = skipped.Take(int.MaxValue); // May try to calculate max index as 42 + int.MaxValue, leading to integer overflow.
            Assert.Equal(Indexerable.Range(43, 100 - 42), taken1);
            Assert.Equal(100 - 42, taken1.Count);
            Assert.Equal(Indexerable.Range(43, 100 - 42), (int[])[.. taken1]);
            Assert.Equal(Indexerable.Range(43, 100 - 42), (List<int>)[.. taken1]);

            IReadOnlyList<int> taken2 = Indexerable.Range(1, 100).Take(42..int.MaxValue);
            Assert.Equal(Indexerable.Range(43, 100 - 42), taken2);
            Assert.Equal(100 - 42, taken2.Count);
            Assert.Equal(Indexerable.Range(43, 100 - 42), (int[])[.. taken2]);
            Assert.Equal(Indexerable.Range(43, 100 - 42), (List<int>)[.. taken2]);

            IReadOnlyList<int> taken3 = Indexerable.Range(1, 100).Take(^(100 - 42)..int.MaxValue);
            Assert.Equal(Indexerable.Range(43, 100 - 42), taken3);
            Assert.Equal(100 - 42, taken3.Count);
            Assert.Equal(Indexerable.Range(43, 100 - 42), (int[])[.. taken3]);
            Assert.Equal(Indexerable.Range(43, 100 - 42), (List<int>)[.. taken3]);

            IReadOnlyList<int> taken4 = Indexerable.Range(1, 100).Take(42..^0);
            Assert.Equal(Indexerable.Range(43, 100 - 42), taken4);
            Assert.Equal(100 - 42, taken4.Count);
            Assert.Equal(Indexerable.Range(43, 100 - 42), (int[])[.. taken4]);
            Assert.Equal(Indexerable.Range(43, 100 - 42), (List<int>)[.. taken4]);

            IReadOnlyList<int> taken5 = Indexerable.Range(1, 100).Take(^(100 - 42)..^0);
            Assert.Equal(Indexerable.Range(43, 100 - 42), taken5);
            Assert.Equal(100 - 42, taken5.Count);
            Assert.Equal(Indexerable.Range(43, 100 - 42), (int[])[.. taken5]);
            Assert.Equal(Indexerable.Range(43, 100 - 42), (List<int>)[.. taken5]);
        }

        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(1, 1, 1)]
        [InlineData(0, int.MaxValue, 100)]
        [InlineData(int.MaxValue, 0, 0)]
        [InlineData(0xffff, 1, 0)]
        [InlineData(1, 0xffff, 99)]
        [InlineData(int.MaxValue, int.MaxValue, 0)]
        [InlineData(1, int.MaxValue, 99)] // Regression test: The max index is precisely int.MaxValue.
        [InlineData(0, 100, 100)]
        [InlineData(10, 100, 90)]
        public void CountOfLazySkipTakeChain(int skip, int take, int expected)
        {
            int totalCount = 100;
            IReadOnlyList<int> partition1 = Indexerable.Range(1, totalCount).Skip(skip).Take(take);
            Assert.Equal(expected, partition1.Count);
            Assert.Equal(expected, partition1.Select(i => i).Count);
            Assert.Equal(expected, partition1.Select(i => i).ToArray().Length);

            int end;
            try
            {
                end = checked(skip + take);
            }
            catch (OverflowException)
            {
                end = int.MaxValue;
            }

            IReadOnlyList<int> partition2 = Indexerable.Range(1, totalCount).Take(skip..end);
            Assert.Equal(expected, partition2.Count);
            Assert.Equal(expected, partition2.Select(i => i).Count);
            Assert.Equal(expected, partition2.Select(i => i).ToArray().Length);

            IReadOnlyList<int> partition3 = Indexerable.Range(1, totalCount).Take(^Math.Max(totalCount - skip, 0)..end);
            Assert.Equal(expected, partition3.Count);
            Assert.Equal(expected, partition3.Select(i => i).Count);
            Assert.Equal(expected, partition3.Select(i => i).ToArray().Length);

            IReadOnlyList<int> partition4 = Indexerable.Range(1, totalCount).Take(skip..^Math.Max(totalCount - end, 0));
            Assert.Equal(expected, partition4.Count);
            Assert.Equal(expected, partition4.Select(i => i).Count);
            Assert.Equal(expected, partition4.Select(i => i).ToArray().Length);

            IReadOnlyList<int> partition5 = Indexerable.Range(1, totalCount).Take(^Math.Max(totalCount - skip, 0)..^Math.Max(totalCount - end, 0));
            Assert.Equal(expected, partition5.Count);
            Assert.Equal(expected, partition5.Select(i => i).Count);
            Assert.Equal(expected, partition5.Select(i => i).ToArray().Length);
        }

        [Fact]
        public void OutOfBoundNoException()
        {
            static int[] source() => [1, 2, 3, 4, 5];

            Assert.Equal(source(), source().Take(0..6));
            Assert.Equal(source(), source().Take(0..int.MaxValue));

            Assert.Equal([1, 2, 3, 4], source().Take(^10..4));
            Assert.Equal([1, 2, 3, 4], source().Take(^int.MaxValue..4));
            Assert.Equal(source(), source().Take(^10..6));
            Assert.Equal(source(), source().Take(^int.MaxValue..6));
            Assert.Equal(source(), source().Take(^10..int.MaxValue));
            Assert.Equal(source(), source().Take(^int.MaxValue..int.MaxValue));

            Assert.Empty(source().Take(0..^6));
            Assert.Empty(source().Take(0..^int.MaxValue));
            Assert.Empty(source().Take(4..^6));
            Assert.Empty(source().Take(4..^int.MaxValue));
            Assert.Empty(source().Take(6..^6));
            Assert.Empty(source().Take(6..^int.MaxValue));
            Assert.Empty(source().Take(int.MaxValue..^6));
            Assert.Empty(source().Take(int.MaxValue..^int.MaxValue));

            Assert.Equal([1, 2, 3, 4], source().Take(^10..^1));
            Assert.Equal([1, 2, 3, 4], source().Take(^int.MaxValue..^1));
            Assert.Empty(source().Take(^0..^6));
            Assert.Empty(source().Take(^1..^6));
            Assert.Empty(source().Take(^6..^6));
            Assert.Empty(source().Take(^10..^6));
            Assert.Empty(source().Take(^int.MaxValue..^6));
            Assert.Empty(source().Take(^0..^int.MaxValue));
            Assert.Empty(source().Take(^1..^int.MaxValue));
            Assert.Empty(source().Take(^6..^int.MaxValue));
            Assert.Empty(source().Take(^int.MaxValue..^int.MaxValue));
        }

        [Fact]
        public void MutableSource()
        {
            List<int> source1 = [0, 1, 2, 3, 4];
            IReadOnlyList<int> query1 = source1.Take(3);
            source1.RemoveAt(0);
            source1.InsertRange(2, [-1, -2]);
            Assert.Equal([1, 2, -1], query1);

            List<int> source2 = [0, 1, 2, 3, 4];
            IReadOnlyList<int> query2 = source2.Take(0..3);
            source2.RemoveAt(0);
            source2.InsertRange(2, [-1, -2]);
            Assert.Equal([1, 2, -1], query2);

            List<int> source3 = [0, 1, 2, 3, 4];
            IReadOnlyList<int> query3 = source3.Take(^6..3);
            source3.RemoveAt(0);
            source3.InsertRange(2, [-1, -2]);
            Assert.Equal([1, 2, -1], query3);

            List<int> source4 = [0, 1, 2, 3, 4];
            IReadOnlyList<int> query4 = source4.Take(^6..^3);
            source4.RemoveAt(0);
            source4.InsertRange(2, [-1, -2]);
            Assert.Equal([1, 2, -1], query4);
        }

        [Fact]
        public void NonEmptySource_ConsistencyWithCountable()
        {
            static int[] source() => [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];

            // Multiple elements in the middle.
            Assert.Equal(source()[^9..5], source().Take(^9..5));
            Assert.Equal(source()[2..7], source().Take(2..7));
            Assert.Equal(source()[2..^4], source().Take(2..^4));
            Assert.Equal(source()[^7..^4], source().Take(^7..^4));

            // Range with default index.
            Assert.Equal(source()[^9..], source().Take(^9..));
            Assert.Equal(source()[2..], source().Take(2..));
            Assert.Equal(source()[..^4], source().Take(..^4));
            Assert.Equal(source()[..6], source().Take(..6));

            // All.
            Assert.Equal(source()[..], source().Take(..));

            // Single element in the middle.
            Assert.Equal(source()[^9..2], source().Take(^9..2));
            Assert.Equal(source()[2..3], source().Take(2..3));
            Assert.Equal(source()[2..^7], source().Take(2..^7));
            Assert.Equal(source()[^5..^4], source().Take(^5..^4));

            // Single element at start.
            Assert.Equal(source()[^10..1], source().Take(^10..1));
            Assert.Equal(source()[0..1], source().Take(0..1));
            Assert.Equal(source()[0..^9], source().Take(0..^9));
            Assert.Equal(source()[^10..^9], source().Take(^10..^9));

            // Single element at end.
            Assert.Equal(source()[^1..10], source().Take(^1..10));
            Assert.Equal(source()[9..10], source().Take(9..10));
            Assert.Equal(source()[9..^0], source().Take(9..^0));
            Assert.Equal(source()[^1..^0], source().Take(^1..^0));

            // No element.
            Assert.Equal(source()[3..3], source().Take(3..3));
            Assert.Equal(source()[6..^4], source().Take(6..^4));
            Assert.Equal(source()[3..^7], source().Take(3..^7));
            Assert.Equal(source()[^3..7], source().Take(^3..7));
            Assert.Equal(source()[^6..^6], source().Take(^6..^6));
        }

        [Fact]
        public void NonEmptySource_DoNotThrowException()
        {
            static int[] source() => [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];

            Assert.Empty(source().Take(3..2));
            Assert.Empty(source().Take(6..^5));
            Assert.Empty(source().Take(3..^8));
            Assert.Empty(source().Take(^6..^7));
        }

        [Fact]
        public void EmptySource_DoNotThrowException()
        {
            static int[] source() => [];

            // Multiple elements in the middle.
            Assert.Empty(source().Take(^9..5));
            Assert.Empty(source().Take(2..7));
            Assert.Empty(source().Take(2..^4));
            Assert.Empty(source().Take(^7..^4));

            // Range with default index.
            Assert.Empty(source().Take(^9..));
            Assert.Empty(source().Take(2..));
            Assert.Empty(source().Take(..^4));
            Assert.Empty(source().Take(..6));

            // All.
            Assert.Equal(source()[..], source().Take(..));

            // Single element in the middle.
            Assert.Empty(source().Take(^9..2));
            Assert.Empty(source().Take(2..3));
            Assert.Empty(source().Take(2..^7));
            Assert.Empty(source().Take(^5..^4));

            // Single element at start.
            Assert.Empty(source().Take(^10..1));
            Assert.Empty(source().Take(0..1));
            Assert.Empty(source().Take(0..^9));
            Assert.Empty(source().Take(^10..^9));

            // Single element at end.
            Assert.Empty(source().Take(^1..^10));
            Assert.Empty(source().Take(9..10));
            Assert.Empty(source().Take(9..^9));
            Assert.Empty(source().Take(^1..^9));

            // No element.
            Assert.Empty(source().Take(3..3));
            Assert.Empty(source().Take(6..^4));
            Assert.Empty(source().Take(3..^7));
            Assert.Empty(source().Take(^3..7));
            Assert.Empty(source().Take(^6..^6));

            // Invalid range.
            Assert.Empty(source().Take(3..2));
            Assert.Empty(source().Take(6..^5));
            Assert.Empty(source().Take(3..^8));
            Assert.Empty(source().Take(^6..^7));
        }
    }
}
