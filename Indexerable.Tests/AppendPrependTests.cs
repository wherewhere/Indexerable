using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace Indexer.Linq.Tests
{
    public sealed class AppendPrependTests : IndexerableTests
    {
        // Mock collection for testing overflow without allocating memory
        private sealed class MockReadOnlyList<T>(int count) : IReadOnlyList<T>
        {
            public T this[int index] => throw new NotImplementedException();
            public int Count => count;
            public IEnumerator<T> GetEnumerator() => Indexerable.Empty<T>().GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Fact]
        public void SameResultsRepeatCallsIntQueryAppend()
        {
            IReadOnlyList<int?> q1 = from x1 in new int?[] { 2, 3, null, 2, null, 4, 5 }
                                     select x1;

            Assert.Equal(q1.Append(42), q1.Append(42));
            Assert.Equal(q1.Append(42), q1.Concat([42]));
        }

        [Fact]
        public void SameResultsRepeatCallsIntQueryPrepend()
        {
            IReadOnlyList<int?> q1 = from x1 in new int?[] { 2, 3, null, 2, null, 4, 5 }
                                     select x1;

            Assert.Equal(q1.Prepend(42), q1.Prepend(42));
            Assert.Equal(q1.Prepend(42), (new int?[] { 42 }).Concat(q1));
        }

        [Fact]
        public void SameResultsRepeatCallsStringQueryAppend()
        {
            IReadOnlyList<string> q1 = from x1 in new[] { "AAA", string.Empty, "q", "C", "#", "!@#$%^", "0987654321", "Calling Twice" }
                                       select x1;

            Assert.Equal(q1.Append("hi"), q1.Append("hi"));
            Assert.Equal(q1.Append("hi"), q1.Concat(["hi"]));
        }

        [Fact]
        public void SameResultsRepeatCallsStringQueryPrepend()
        {
            IReadOnlyList<string> q1 = from x1 in new[] { "AAA", string.Empty, "q", "C", "#", "!@#$%^", "0987654321", "Calling Twice" }
                                       select x1;

            Assert.Equal(q1.Prepend("hi"), q1.Prepend("hi"));
            Assert.Equal(q1.Prepend("hi"), ((string[])["hi"]).Concat(q1));
        }

        [Fact]
        public void RepeatIteration()
        {
            IReadOnlyList<int> q = Indexerable.Range(3, 4).Append(12);
            Assert.Equal(q, q);
            q = q.Append(14);
            Assert.Equal(q, q);
        }

        [Fact]
        public void EmptyAppend()
        {
            int[] first = [];
            Assert.All(CreateSources(first), first =>
            {
                Assert.Single(first.Append(42), 42);
            });
        }

        [Fact]
        public void EmptyPrepend()
        {
            string[] first = [];
            Assert.All(CreateSources(first), first =>
            {
                Assert.Single(first.Prepend("aa"), "aa");
            });
        }

        [Fact]
        public void PrependNoIteratingSourceBeforeFirstItem()
        {
            List<int> ie = [];
            IReadOnlyList<int> prepended = (from i in ie select i).Prepend(4);

            ie.Add(42);

            Assert.Equal(prepended, ie.Prepend(4));
        }

        [Fact]
        public void ForcedToEnumeratorDoesntEnumeratePrepend()
        {
            IReadOnlyList<int> iterator = Indexerable.Range(0, 3).Prepend(4);
            // Don't insist on this behaviour, but check it's correct if it happens
            IEnumerator<int> en = iterator as IEnumerator<int>;
            Assert.False(en is not null && en.MoveNext());
        }

        [Fact]
        public void ForcedToEnumeratorDoesntEnumerateAppend()
        {
            IReadOnlyList<int> iterator = Indexerable.Range(0, 3).Append(4);
            // Don't insist on this behaviour, but check it's correct if it happens
            IEnumerator<int> en = iterator as IEnumerator<int>;
            Assert.False(en is not null && en.MoveNext());
        }

        [Fact]
        public void ForcedToEnumeratorDoesntEnumerateMultipleAppendsAndPrepends()
        {
            IReadOnlyList<int> iterator = Indexerable.Range(0, 3).Append(4).Append(5).Prepend(-1).Prepend(-2);
            // Don't insist on this behaviour, but check it's correct if it happens
            IEnumerator<int> en = iterator as IEnumerator<int>;
            Assert.False(en is not null && en.MoveNext());
        }

        [Fact]
        public void SourceNull()
        {
            AssertExtensions.Throws<ArgumentNullException>("source", () => ((IReadOnlyList<int>)null).Append(1));
            AssertExtensions.Throws<ArgumentNullException>("source", () => ((IReadOnlyList<int>)null).Prepend(1));
        }

        [Fact]
        public void Combined()
        {
            IReadOnlyList<char> v = "foo".ToReadOnlyList().Append('1').Append('2').Prepend('3').Concat("qq".ToReadOnlyList().Append('Q').Prepend('W'));

            Assert.Equal(v, [.. "3foo12WqqQ"]);

            IReadOnlyList<char> v1 = "a".ToReadOnlyList().Append('b').Append('c').Append('d');

            Assert.Equal(v1, [.. "abcd"]);

            IReadOnlyList<char> v2 = "a".ToReadOnlyList().Prepend('b').Prepend('c').Prepend('d');

            Assert.Equal(v2, [.. "dcba"]);
        }

        [Fact]
        public void AppendCombinations()
        {
            IReadOnlyList<int> source = Indexerable.Range(0, 3).Append(3).Append(4);
            IReadOnlyList<int> app0a = source.Append(5);
            IReadOnlyList<int> app0b = source.Append(6);
            IReadOnlyList<int> app1aa = app0a.Append(7);
            IReadOnlyList<int> app1ab = app0a.Append(8);
            IReadOnlyList<int> app1ba = app0b.Append(9);
            IReadOnlyList<int> app1bb = app0b.Append(10);

            Assert.Equal((int[])[0, 1, 2, 3, 4, 5], app0a);
            Assert.Equal((int[])[0, 1, 2, 3, 4, 6], app0b);
            Assert.Equal((int[])[0, 1, 2, 3, 4, 5, 7], app1aa);
            Assert.Equal((int[])[0, 1, 2, 3, 4, 5, 8], app1ab);
            Assert.Equal((int[])[0, 1, 2, 3, 4, 6, 9], app1ba);
            Assert.Equal((int[])[0, 1, 2, 3, 4, 6, 10], app1bb);
        }

        [Fact]
        public void PrependCombinations()
        {
            IReadOnlyList<int> source = Indexerable.Range(2, 2).Prepend(1).Prepend(0);
            IReadOnlyList<int> pre0a = source.Prepend(5);
            IReadOnlyList<int> pre0b = source.Prepend(6);
            IReadOnlyList<int> pre1aa = pre0a.Prepend(7);
            IReadOnlyList<int> pre1ab = pre0a.Prepend(8);
            IReadOnlyList<int> pre1ba = pre0b.Prepend(9);
            IReadOnlyList<int> pre1bb = pre0b.Prepend(10);

            Assert.Equal((int[])[5, 0, 1, 2, 3], pre0a);
            Assert.Equal((int[])[6, 0, 1, 2, 3], pre0b);
            Assert.Equal((int[])[7, 5, 0, 1, 2, 3], pre1aa);
            Assert.Equal((int[])[8, 5, 0, 1, 2, 3], pre1ab);
            Assert.Equal((int[])[9, 6, 0, 1, 2, 3], pre1ba);
            Assert.Equal((int[])[10, 6, 0, 1, 2, 3], pre1bb);
        }

        [Fact]
        public void Append1ToArrayToList()
        {
            Assert.All(CreateSources(Indexerable.Range(0, 2)), source =>
            {
                source = source.Append(2);
                Assert.Equal(Indexerable.Range(0, 3), source);
            });
        }

        [Fact]
        public void Prepend1ToArrayToList()
        {
            Assert.All(CreateSources(Indexerable.Range(1, 2)), source =>
            {
                source = source.Prepend(0);
                Assert.Equal(Indexerable.Range(0, 3), source);
            });
        }

        [Fact]
        public void AppendNToArrayToList()
        {
            Assert.All(CreateSources(Indexerable.Range(0, 2)), source =>
            {
                source = source.Append(2).Append(3);
                Assert.Equal(Indexerable.Range(0, 4), source);
            });
        }

        [Fact]
        public void PrependNToArrayToList()
        {
            Assert.All(CreateSources(Indexerable.Range(2, 2)), source =>
            {
                source = source.Prepend(1).Prepend(0);
                Assert.Equal(Indexerable.Range(0, 4), source);
            });
        }

        [Fact]
        public void AppendPrependToArrayToList()
        {
            Assert.All(CreateSources(Indexerable.Range(2, 2)), source =>
            {
                source = source.Prepend(1).Append(4).Prepend(0).Append(5);
                Assert.Equal(Indexerable.Range(0, 6), source);
            });
        }

        [Fact]
        public void TakeAfterMultiplePrepends()
        {
            IReadOnlyList<int> prepended = Indexerable.Range(0, 3).Prepend(-2).Prepend(-1);
            Assert.Equal([-1, -2, 0, 1], prepended.Take(4));

            IReadOnlyList<int> combined = prepended.Append(3).Append(4);
            Assert.Equal([-1, -2, 0, 1, 2, 3], combined.Take(6));
        }

        [Fact]
        public void AppendPrepend_First_Last_ElementAt()
        {
            Assert.Equal(42, ((int[])[42]).Append(84)[0]);
            Assert.Equal(42, ((int[])[84]).Prepend(42)[0]);
            Assert.Equal(84, ((int[])[42]).Append(84)[^1]);
            Assert.Equal(84, ((int[])[84]).Prepend(42)[^1]);
            Assert.Equal(42, ((int[])[42]).Append(84)[0]);
            Assert.Equal(42, ((int[])[84]).Prepend(42)[0]);
            Assert.Equal(84, ((int[])[42]).Append(84)[1]);
            Assert.Equal(84, ((int[])[84]).Prepend(42)[1]);

            Assert.Equal(42, Indexerable.Range(42, 1).Append(84)[0]);
            Assert.Equal(42, Indexerable.Range(84, 1).Prepend(42)[0]);
            Assert.Equal(84, Indexerable.Range(42, 1).Append(84)[^1]);
            Assert.Equal(84, Indexerable.Range(84, 1).Prepend(42)[^1]);
            Assert.Equal(42, Indexerable.Range(42, 1).Append(84)[0]);
            Assert.Equal(42, Indexerable.Range(84, 1).Prepend(42)[0]);
            Assert.Equal(84, Indexerable.Range(42, 1).Append(84)[1]);
            Assert.Equal(84, Indexerable.Range(84, 1).Prepend(42)[1]);
        }

        [Fact]
        public void AppendOverflowCount()
        {
            // AppendPrepend1Iterator overflow when source has GetCount optimization
            IReadOnlyList<int> source = Indexerable.Repeat(0, int.MaxValue);
            IReadOnlyList<int> appended = source.Append(1);
            Assert.Throws<OverflowException>(() => appended.Count);
        }

        [Fact]
        public void PrependOverflowCount()
        {
            // AppendPrepend1Iterator overflow when source has GetCount optimization
            IReadOnlyList<int> source = Indexerable.Repeat(0, int.MaxValue);
            IReadOnlyList<int> prepended = source.Prepend(1);
            Assert.Throws<OverflowException>(() => prepended.Count);
        }

        [Fact]
        public void AppendPrependNOverflowCount()
        {
            // AppendPrependN overflow when source has GetCount optimization
            IReadOnlyList<int> source = Indexerable.Repeat(0, int.MaxValue);
            IReadOnlyList<int> result = source.Append(1).Prepend(2);
            Assert.Throws<OverflowException>(() => result.Count);
        }

        [Fact]
        public void AppendOverflowCountWithICollection()
        {
            // AppendPrepend1Iterator overflow when source is ICollection
            MockReadOnlyList<int> source = new(int.MaxValue);
            IReadOnlyList<int> appended = source.Append(1);
            Assert.Throws<OverflowException>(() => appended.Count);
        }

        [Fact]
        public void PrependOverflowCountWithICollection()
        {
            // AppendPrepend1Iterator overflow when source is ICollection
            MockReadOnlyList<int> source = new(int.MaxValue);
            IReadOnlyList<int> prepended = source.Prepend(1);
            Assert.Throws<OverflowException>(() => prepended.Count);
        }

        [Fact]
        public void AppendPrependNOverflowCountWithICollection()
        {
            // AppendPrependN overflow when source is ICollection
            MockReadOnlyList<int> source = new(int.MaxValue);
            IReadOnlyList<int> result = source.Append(1).Prepend(2);
            Assert.Throws<OverflowException>(() => result.Count);
        }
    }
}
