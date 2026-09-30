using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Indexer.Linq.Tests
{
    public class SkipTests : IndexerableTests
    {
        [Fact]
        public void SkipSome()
        {
            Assert.Equal(Indexerable.Range(10, 10), Indexerable.Range(0, 20).Skip(10));
        }

        [Fact]
        public void SkipNone()
        {
            Assert.Equal(Indexerable.Range(0, 20), Indexerable.Range(0, 20).Skip(0));
        }

        [Fact]
        public void SkipExcessive()
        {
            Assert.Equal([], Indexerable.Range(0, 20).Skip(42));
        }

        [Fact]
        public void SkipAllExactly()
        {
            Assert.False(Indexerable.Range(0, 20).Skip(20).Any());
        }

        [Fact]
        public void SkipThrowsOnNull()
        {
            AssertExtensions.Throws<ArgumentNullException>("source", () => ((IReadOnlyList<DateTime>)null).Skip(3));
        }

        [Fact]
        public void SkipThrowsOnNullIList()
        {
            AssertExtensions.Throws<ArgumentNullException>("source", () => ((List<DateTime>)null).Skip(3));
        }

        [Fact]
        public void SkipOnEmpty()
        {
            foreach (IReadOnlyList<int> source in CreateSources<int>([]))
            {
                Assert.Equal([], source.Skip(0));
                Assert.Equal([], source.Skip(-1));
                Assert.Equal([], source.Skip(1));
            }

            foreach (IReadOnlyList<string> source in CreateSources<string>([]))
            {
                Assert.Equal([], source.Skip(0));
                Assert.Equal([], source.Skip(-1));
                Assert.Equal([], source.Skip(1));
            }
        }

        [Fact]
        public void SkipNegative()
        {
            foreach (IReadOnlyList<int> source in CreateSources(Indexerable.Range(0, 20)))
            {
                Assert.Equal(Indexerable.Range(0, 20), source.Skip(-42));
            }
        }

        [Fact]
        public void SameResultsRepeatCallsIntQuery()
        {
            foreach (IReadOnlyList<int> source in CreateSources([9999, 0, 888, -1, 66, -777, 1, 2, -12345]))
            {
                IReadOnlyList<int> q = from x in source
                                       select x;

                Assert.Equal(q.Skip(0), q.Skip(0));
            }
        }

        [Fact]
        public void SameResultsRepeatCallsStringQuery()
        {
            foreach (IReadOnlyList<string> source in CreateSources(["!@#$%^", "C", "AAA", "", "Calling Twice", "SoS", string.Empty]))
            {
                IReadOnlyList<string> q = from x in source
                                          select x;

                Assert.Equal(q.Skip(0), q.Skip(0));
            }
        }

        [Fact]
        public void SkipOne()
        {
            int?[] expected = [100, 4, null, 10];
            foreach (IReadOnlyList<int?> source in CreateSources<int?>([3, 100, 4, null, 10]))
            {
                Assert.Equal(expected, source.Skip(1));
            }
        }

        [Fact]
        public void SkipAllButOne()
        {
            int?[] expected = [10];
            foreach (IReadOnlyList<int?> source in CreateSources<int?>([3, 100, 4, null, 10]))
            {
                Assert.Equal(expected, source.Skip(4));
            }
        }

        [Fact]
        public void SkipOneMoreThanAll()
        {
            foreach (IReadOnlyList<int> source in CreateSources([3, 100, 4, 10]))
            {
                Assert.Empty(source.Skip(5));
            }
        }

        [Fact]
        public void ForcedToEnumeratorDoesntEnumerate()
        {
            foreach (IReadOnlyList<int> source in CreateSources(Indexerable.Range(0, 3)))
            {
                // Don't insist on this behaviour, but check it's correct if it happens
                IReadOnlyList<int> iterator = source.Skip(2);
                IEnumerator<int> en = iterator as IEnumerator<int>;
                Assert.False(en is not null && en.MoveNext());
            }
        }

        [Fact]
        public void Count()
        {
            Assert.Equal(2, Indexerable.Range(0, 3).Skip(1).Count);
            Assert.Equal(2, ((int[])[1, 2, 3]).Skip(1).Count);
        }

        [Fact]
        public void FollowWithTake()
        {
            int[] expected = [6, 7];
            foreach (IReadOnlyList<int> source in CreateSources(Indexerable.Range(5, 4)))
            {
                Assert.Equal(expected, source.Skip(1).Take(2));
            }
        }

        [Fact]
        public void FollowWithTakeThenMassiveTake()
        {
            int[] expected = [7];
            foreach (IReadOnlyList<int> source in CreateSources([5, 6, 7, 8]))
            {
                Assert.Equal(expected, source.Skip(2).Take(1).Take(int.MaxValue));
            }
        }

        [Fact]
        public void FollowWithSkip()
        {
            int[] expected = [4, 5, 6];
            foreach (IReadOnlyList<int> source in CreateSources([1, 2, 3, 4, 5, 6]))
            {
                Assert.Equal(expected, source.Skip(1).Skip(2).Skip(-4));
            }
        }

        [Fact]
        public void ElementAt()
        {
            foreach (IReadOnlyList<int> source in CreateSources([1, 2, 3, 4, 5, 6]))
            {
                IReadOnlyList<int> remaining = source.Skip(2);
                Assert.Equal(3, remaining[0]);
                Assert.Equal(4, remaining[1]);
                Assert.Equal(6, remaining[3]);
                AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => _ = remaining[-1]);
                AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => _ = remaining[4]);
            }
        }

        [Fact]
        public void First()
        {
            foreach (IReadOnlyList<int> source in CreateSources([1, 2, 3, 4, 5]))
            {
                Assert.Equal(1, source.Skip(0)[0]);
                Assert.Equal(3, source.Skip(2)[0]);
                Assert.Equal(5, source.Skip(4)[0]);
                Assert.Throws<ArgumentOutOfRangeException>(() => _ = source.Skip(5)[0]);
            }
        }

        [Fact]
        public void Last()
        {
            foreach (IReadOnlyList<int> source in CreateSources([1, 2, 3, 4, 5]))
            {
                Assert.Equal(5, source.Skip(0)[^1]);
                Assert.Equal(5, source.Skip(1)[^1]);
                Assert.Equal(5, source.Skip(4)[^1]);
                Assert.Throws<ArgumentOutOfRangeException>(() => _ = source.Skip(5)[^1]);
            }
        }

        [Fact]
        public void ToArray()
        {
            foreach (IReadOnlyList<int> source in CreateSources([1, 2, 3, 4, 5]))
            {
                Assert.Equal((int[])[1, 2, 3, 4, 5], source.Skip(0).ToArray());
                Assert.Equal((int[])[2, 3, 4, 5], source.Skip(1).ToArray());
                Assert.Equal(5, source.Skip(4).ToArray().Single());
                Assert.Empty(source.Skip(5).ToArray());
                Assert.Empty(source.Skip(40).ToArray());
            }
        }

        [Fact]
        public void ToList()
        {
            foreach (IReadOnlyList<int> source in CreateSources([1, 2, 3, 4, 5]))
            {
                Assert.Equal((int[])[1, 2, 3, 4, 5], source.Skip(0).ToList());
                Assert.Equal((int[])[2, 3, 4, 5], source.Skip(1).ToList());
                Assert.Equal(5, source.Skip(4).ToList().Single());
                Assert.Empty(source.Skip(5).ToList());
                Assert.Empty(source.Skip(40).ToList());
            }
        }

        [Fact]
        public void RepeatEnumerating()
        {
            foreach (IReadOnlyList<int> source in CreateSources([1, 2, 3, 4, 5]))
            {
                IReadOnlyList<int> remaining = source.Skip(1);
                Assert.Equal(remaining, remaining);
            }
        }

        [Fact]
        public void LazySkipMoreThan32Bits()
        {
            IReadOnlyList<int> range = Indexerable.Range(1, 100);
            IReadOnlyList<int> skipped = range.Skip(50).Skip(int.MaxValue); // Could cause an integer overflow.
            Assert.Empty(skipped);
            Assert.Empty(skipped.ToArray());
            Assert.Empty(skipped.ToList());
        }

        [Fact]
        public void SkipMoreThanCountFollowedByOperators()
        {
            int[] items = [2, 3];

            foreach (IReadOnlyList<int> source in CreateSources([1]))
            {
                Assert.Equal(items, source.Skip(2).Concat(items));
                Assert.Equal(items, source.Skip(2).Append(2).Append(3));
                Assert.Equal(items, items.Concat(source.Skip(2)));
                Assert.Empty(source.Skip(2).Select(x => x * 2));
                Assert.Empty(source.Skip(2).Take(10));
                Assert.Empty(source.Skip(2).Skip(1));
            }
        }
    }
}
