using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xunit;

namespace Indexer.Linq.Tests
{
    public class SelectTests : IndexerableTests
    {
        [Fact]
        public void SingleElement()
        {
            var source = new[]
            {
                new { name = "Prakash", custID = 98088 }
            };
            string[] expected = ["Prakash"];

            Assert.Equal(expected, source.Select(e => e.name));
        }

        [Fact]
        public void SelectProperty()
        {
            var source = new[]{
                new { name="Prakash", custID=98088 },
                new { name="Bob", custID=29099 },
                new { name="Chris", custID=39033 },
                new { name=(string)null, custID=30349 },
                new { name="Prakash", custID=39030 }
            };
            string[] expected = ["Prakash", "Bob", "Chris", null, "Prakash"];
            Assert.Equal(expected, source.Select(e => e.name));
        }

        [Fact]
        public void EmptyWithIndexedSelector()
        {
            Assert.Equal([], Indexerable.Empty<string>().Select((s, i) => s.Length + i));
        }

        [Fact]
        public void SingleElementIndexedSelector()
        {
            var source = new[]
            {
                new  { name = "Prakash", custID = 98088 }
            };
            string[] expected = ["Prakash"];

            Assert.Equal(expected, source.Select((e, index) => e.name));
        }

        [Fact]
        public void SelectPropertyPassingIndex()
        {
            var source = new[]{
                new { name="Prakash", custID=98088 },
                new { name="Bob", custID=29099 },
                new { name="Chris", custID=39033 },
                new { name=(string)null, custID=30349 },
                new { name="Prakash", custID=39030 }
            };
            string[] expected = ["Prakash", "Bob", "Chris", null, "Prakash"];
            Assert.Equal(expected, source.Select((e, i) => e.name));
        }

        [Fact]
        public void SelectPropertyUsingIndex()
        {
            var source = new[]{
                new { name="Prakash", custID=98088 },
                new { name="Bob", custID=29099 },
                new { name="Chris", custID=39033 }
            };
            string[] expected = ["Prakash", null, null];
            Assert.Equal(expected, source.Select((e, i) => i == 0 ? e.name : null));
        }

        [Fact]
        public void SelectPropertyPassingIndexOnLast()
        {
            var source = new[]{
                new { name="Prakash", custID=98088},
                new { name="Bob", custID=29099 },
                new { name="Chris", custID=39033 },
                new { name="Robert", custID=39033 },
                new { name="Allen", custID=39033 },
                new { name="Chuck", custID=39033 }
            };
            string[] expected = [null, null, null, null, null, "Chuck"];
            Assert.Equal(expected, source.Select((e, i) => i == 5 ? e.name : null));
        }

        [Fact]
        public void Select_SourceIsNull_ArgumentNullExceptionThrown()
        {
            IReadOnlyList<int> source = null;
            static int selector(int i) => i + 1;

            AssertExtensions.Throws<ArgumentNullException>("source", () => source.Select(selector));
        }

        [Fact]
        public void Select_SelectorIsNull_ArgumentNullExceptionThrown_Indexed()
        {
            IReadOnlyList<int> source = Indexerable.Range(1, 10);
            Func<int, int, int> selector = null;

            AssertExtensions.Throws<ArgumentNullException>("selector", () => source.Select(selector));
        }

        [Fact]
        public void Select_SourceIsNull_ArgumentNullExceptionThrown_Indexed()
        {
            IReadOnlyList<int> source = null;
            static int selector(int e, int i) => i + 1;

            AssertExtensions.Throws<ArgumentNullException>("source", () => source.Select(selector));
        }

        [Fact]
        public void Select_SelectorIsNull_ArgumentNullExceptionThrown()
        {
            IReadOnlyList<int> source = Indexerable.Range(1, 10);
            Func<int, int> selector = null;

            AssertExtensions.Throws<ArgumentNullException>("selector", () => source.Select(selector));
        }

        [Fact]
        public void Select_SourceIsAnArray_ExecutionIsDeferred()
        {
            bool funcCalled = false;
            Func<int>[] source = [() => { funcCalled = true; return 1; }];

            IReadOnlyList<int> query = source.Select(d => d());
            Assert.False(funcCalled);
        }

        [Fact]
        public void Select_SourceIsAList_ExecutionIsDeferred()
        {
            bool funcCalled = false;
            List<Func<int>> source =
            [
                () =>
                {
                    funcCalled = true;
                    return 1;
                }
            ];

            IReadOnlyList<int> query = source.Select(d => d());
            Assert.False(funcCalled);
        }

        [Fact]
        public void Select_SourceIsIReadOnlyList_ExecutionIsDeferred()
        {
            bool funcCalled = false;
            IReadOnlyList<Func<int>> source = Indexerable.Repeat(() => { funcCalled = true; return 1; }, 1);

            IReadOnlyList<int> query = source.Select(d => d());
            Assert.False(funcCalled);
        }

        [Fact]
        public void SelectSelect_SourceIsAnArray_ExecutionIsDeferred()
        {
            bool funcCalled = false;
            Func<int>[] source = [() => { funcCalled = true; return 1; }];

            IReadOnlyList<int> query = source.Select(d => d).Select(d => d());
            Assert.False(funcCalled);
        }

        [Fact]
        public void SelectSelect_SourceIsAList_ExecutionIsDeferred()
        {
            bool funcCalled = false;
            List<Func<int>> source =
            [
                () =>
                {
                    funcCalled = true;
                    return 1;
                }
            ];

            IReadOnlyList<int> query = source.Select(d => d).Select(d => d());
            Assert.False(funcCalled);
        }

        [Fact]
        public void SelectSelect_SourceIsIReadOnlyList_ExecutionIsDeferred()
        {
            bool funcCalled = false;
            IReadOnlyList<Func<int>> source = Indexerable.Repeat(() => { funcCalled = true; return 1; }, 1);

            IReadOnlyList<int> query = source.Select(d => d).Select(d => d());
            Assert.False(funcCalled);
        }

        [Fact]
        public void Select_SourceIsAnArray_ReturnsExpectedValues()
        {
            int[] source = [1, 2, 3, 4, 5];
            static int selector(int i) => i + 1;

            IReadOnlyList<int> query = source.Select(selector);

            int index = 0;
            foreach (int item in query)
            {
                int expected = selector(source[index]);
                Assert.Equal(expected, item);
                index++;
            }

            Assert.Equal(source.Length, index);
        }

        [Fact]
        public void Select_SourceIsAList_ReturnsExpectedValues()
        {
            List<int> source = [1, 2, 3, 4, 5];
            static int selector(int i) => i + 1;

            IReadOnlyList<int> query = source.Select(selector);

            int index = 0;
            foreach (int item in query)
            {
                int expected = selector(source[index]);
                Assert.Equal(expected, item);
                index++;
            }

            Assert.Equal(source.Count, index);
        }

        [Fact]
        public void Select_SourceIsIReadOnlyList_ReturnsExpectedValues()
        {
            int nbOfItems = 5;
            IReadOnlyList<int> source = Indexerable.Range(1, nbOfItems);
            static int selector(int i) => i + 1;

            IReadOnlyList<int> query = source.Select(selector);

            int index = 0;
            foreach (int item in query)
            {
                index++;
                int expected = selector(index);
                Assert.Equal(expected, item);
            }

            Assert.Equal(nbOfItems, index);
        }

        [Fact]
        public void SelectSelect_SourceIsAnArray_ReturnsExpectedValues()
        {
            static int selector(int i) => i + 1;
            int[] source = [1, 2, 3, 4, 5];

            IReadOnlyList<int> query = source.Select(selector).Select(selector);

            int index = 0;
            foreach (int item in query)
            {
                int expected = selector(selector(source[index]));
                Assert.Equal(expected, item);
                index++;
            }

            Assert.Equal(source.Length, index);
        }

        [Fact]
        public void SelectSelect_SourceIsAList_ReturnsExpectedValues()
        {
            List<int> source = [1, 2, 3, 4, 5];
            static int selector(int i) => i + 1;

            IReadOnlyList<int> query = source.Select(selector).Select(selector);

            int index = 0;
            foreach (int item in query)
            {
                int expected = selector(selector(source[index]));
                Assert.Equal(expected, item);
                index++;
            }

            Assert.Equal(source.Count, index);
        }

        [Fact]
        public void SelectSelect_SourceIsIReadOnlyList_ReturnsExpectedValues()
        {
            int nbOfItems = 5;
            IReadOnlyList<int> source = Indexerable.Range(1, 5);
            static int selector(int i) => i + 1;

            IReadOnlyList<int> query = source.Select(selector).Select(selector);

            int index = 0;
            foreach (int item in query)
            {
                index++;
                int expected = selector(selector(index));
                Assert.Equal(expected, item);
            }

            Assert.Equal(nbOfItems, index);
        }

        [Fact]
        public void Select_SourceIsEmptyIndexerable_ReturnedCollectionHasNoElements()
        {
            IReadOnlyList<int> source = [];
            bool wasSelectorCalled = false;

            IReadOnlyList<int> result = source.Select(i => { wasSelectorCalled = true; return i + 1; });

            bool hadItems = false;
            foreach (int item in result)
            {
                hadItems = true;
            }

            Assert.False(hadItems);
            Assert.False(wasSelectorCalled);
        }

        [Fact]
        public void Select_ExceptionThrownFromSelector_ExceptionPropagatedToTheCaller()
        {
            int[] source = [1, 2, 3, 4, 5];
            static int selector(int i) { throw new InvalidOperationException(); }

            IReadOnlyList<int> result = source.Select(selector);
            using IEnumerator<int> enumerator = result.GetEnumerator();

            enumerator.MoveNext();
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);
        }

        [Fact]
        public void Select_ExceptionThrownFromSelector_IteratorCanBeUsedAfterExceptionIsCaught()
        {
            int[] source = [1, 2, 3, 4, 5];
            static int selector(int i)
            {
                return i == 1 ? throw new InvalidOperationException() : i + 1;
            }

            IReadOnlyList<int> result = source.Select(selector);
            using IEnumerator<int> enumerator = result.GetEnumerator();

            enumerator.MoveNext();
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);
            enumerator.MoveNext();
            Assert.Equal(3 /* 2 + 1 */, enumerator.Current);
        }

        [Fact]
        public void ForcedToEnumeratorDoesntEnumerate()
        {
            IReadOnlyList<int> iterator = Indexerable.Range(0, 3).Select(i => i);
            // Don't insist on this behaviour, but check it's correct if it happens
            IEnumerator<int> en = iterator as IEnumerator<int>;
            Assert.False(en is not null && en.MoveNext());
        }

        [Fact]
        public void ForcedToEnumeratorDoesntEnumerateIndexed()
        {
            IReadOnlyList<int> iterator = Indexerable.Range(0, 3).Select((e, i) => i);
            // Don't insist on this behaviour, but check it's correct if it happens
            IEnumerator<int> en = iterator as IEnumerator<int>;
            Assert.False(en is not null && en.MoveNext());
        }

        [Fact]
        public void ForcedToEnumeratorDoesntEnumerateArray()
        {
            IReadOnlyList<int> iterator = Indexerable.Range(0, 3).ToArray().Select(i => i);
            IEnumerator<int> en = iterator as IEnumerator<int>;
            Assert.False(en is not null && en.MoveNext());
        }

        [Fact]
        public void ForcedToEnumeratorDoesntEnumerateList()
        {
            IReadOnlyList<int> iterator = Indexerable.Range(0, 3).ToList().Select(i => i);
            IEnumerator<int> en = iterator as IEnumerator<int>;
            Assert.False(en is not null && en.MoveNext());
        }

        [Fact]
        public void ForcedToEnumeratorDoesntEnumerateIList()
        {
            IReadOnlyList<int> iterator = Indexerable.Range(0, 3).ToList().AsReadOnly().Select(i => i);
            IEnumerator<int> en = iterator as IEnumerator<int>;
            Assert.False(en is not null && en.MoveNext());
        }

        [Fact]
        public void ForcedToEnumeratorDoesntEnumerateIPartition()
        {
            IReadOnlyList<int> iterator = Indexerable.Range(0, 3).ToList().AsReadOnly().Select(i => i).Skip(1);
            IEnumerator<int> en = iterator as IEnumerator<int>;
            Assert.False(en is not null && en.MoveNext());
        }

        [Fact]
        public void Select_SourceIsArray_Count()
        {
            int[] source = [1, 2, 3, 4];
            Assert.Equal(source.Length, source.Select(i => i * 2).Count);
        }

        [Fact]
        public void Select_SourceIsAList_Count()
        {
            List<int> source = [1, 2, 3, 4];
            Assert.Equal(source.Count, source.Select(i => i * 2).Count);
        }

        [Fact]
        public void Select_SourceIsAnIList_Count()
        {
            ReadOnlyCollection<int> source = ((List<int>)[1, 2, 3, 4]).AsReadOnly();
            Assert.Equal(source.Count, source.Select(i => i * 2).Count);
        }

        [Fact]
        public void Select_SourceIsArray_Skip()
        {
            IReadOnlyList<int> source = ((int[])[1, 2, 3, 4]).Select(i => i * 2);
            Assert.Equal([6, 8], source.Skip(2));
            Assert.Equal([6, 8], source.Skip(2).Skip(-1));
            Assert.Equal([6, 8], source.Skip(1).Skip(1));
            Assert.Equal([2, 4, 6, 8], source.Skip(-1));
            Assert.Empty(source.Skip(4));
            Assert.Empty(source.Skip(20));
        }

        [Fact]
        public void Select_SourceIsList_Skip()
        {
            IReadOnlyList<int> source = ((List<int>)[1, 2, 3, 4]).Select(i => i * 2);
            Assert.Equal([6, 8], source.Skip(2));
            Assert.Equal([6, 8], source.Skip(2).Skip(-1));
            Assert.Equal([6, 8], source.Skip(1).Skip(1));
            Assert.Equal([2, 4, 6, 8], source.Skip(-1));
            Assert.Empty(source.Skip(4));
            Assert.Empty(source.Skip(20));
        }

        [Fact]
        public void Select_SourceIsIList_Skip()
        {
            IReadOnlyList<int> source = ((List<int>)[1, 2, 3, 4]).AsReadOnly().Select(i => i * 2);
            Assert.Equal([6, 8], source.Skip(2));
            Assert.Equal([6, 8], source.Skip(2).Skip(-1));
            Assert.Equal([6, 8], source.Skip(1).Skip(1));
            Assert.Equal([2, 4, 6, 8], source.Skip(-1));
            Assert.Empty(source.Skip(4));
            Assert.Empty(source.Skip(20));
        }

        [Fact]
        public void Select_SourceIsArray_Take()
        {
            IReadOnlyList<int> source = ((int[])[1, 2, 3, 4]).Select(i => i * 2);
            Assert.Equal([2, 4], source.Take(2));
            Assert.Equal([2, 4], source.Take(3).Take(2));
            Assert.Empty(source.Take(-1));
            Assert.Equal([2, 4, 6, 8], source.Take(4));
            Assert.Equal([2, 4, 6, 8], source.Take(40));
            Assert.Equal([2], source.Take(1));
            Assert.Equal([4], source.Skip(1).Take(1));
            Assert.Equal([6], source.Take(3).Skip(2));
            Assert.Equal([2], source.Take(3).Take(1));
        }

        [Fact]
        public void Select_SourceIsList_Take()
        {
            IReadOnlyList<int> source = ((List<int>)[1, 2, 3, 4]).Select(i => i * 2);
            Assert.Equal([2, 4], source.Take(2));
            Assert.Equal([2, 4], source.Take(3).Take(2));
            Assert.Empty(source.Take(-1));
            Assert.Equal([2, 4, 6, 8], source.Take(4));
            Assert.Equal([2, 4, 6, 8], source.Take(40));
            Assert.Equal([2], source.Take(1));
            Assert.Equal([4], source.Skip(1).Take(1));
            Assert.Equal([6], source.Take(3).Skip(2));
            Assert.Equal([2], source.Take(3).Take(1));
        }

        [Fact]
        public void Select_SourceIsIList_Take()
        {
            IReadOnlyList<int> source = ((List<int>)[1, 2, 3, 4]).AsReadOnly().Select(i => i * 2);
            Assert.Equal([2, 4], source.Take(2));
            Assert.Equal([2, 4], source.Take(3).Take(2));
            Assert.Empty(source.Take(-1));
            Assert.Equal([2, 4, 6, 8], source.Take(4));
            Assert.Equal([2, 4, 6, 8], source.Take(40));
            Assert.Equal([2], source.Take(1));
            Assert.Equal([4], source.Skip(1).Take(1));
            Assert.Equal([6], source.Take(3).Skip(2));
            Assert.Equal([2], source.Take(3).Take(1));
        }

        [Fact]
        public void Select_SourceIsArray_ElementAt()
        {
            IReadOnlyList<int> source = ((int[])[1, 2, 3, 4]).Select(i => i * 2);
            for (int i = 0; i != 4; ++i)
            {
                Assert.Equal((i * 2) + 2, source[i]);
            }

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => _ = source[-1]);
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => _ = source[4]);
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => _ = source[40]);

            Assert.Equal(6, source.Skip(1)[1]);
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => _ = source.Skip(2)[9]);
        }

        [Fact]
        public void Select_SourceIsList_ElementAt()
        {
            IReadOnlyList<int> source = ((List<int>)[1, 2, 3, 4]).Select(i => i * 2);
            for (int i = 0; i != 4; ++i)
            {
                Assert.Equal((i * 2) + 2, source[i]);
            }

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => _ = source[-1]);
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => _ = source[4]);
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => _ = source[40]);

            Assert.Equal(6, source.Skip(1)[1]);
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => _ = source.Skip(2)[9]);
        }

        [Fact]
        public void Select_SourceIsIList_ElementAt()
        {
            IReadOnlyList<int> source = ((List<int>)[1, 2, 3, 4]).AsReadOnly().Select(i => i * 2);
            for (int i = 0; i != 4; ++i)
            {
                Assert.Equal((i * 2) + 2, source[i]);
            }

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => _ = source[-1]);
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => _ = source[4]);
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => _ = source[40]);

            Assert.Equal(6, source.Skip(1)[1]);
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => _ = source.Skip(2)[9]);
        }

        [Fact]
        public void Select_SourceIsArray_First()
        {
            IReadOnlyList<int> source = ((int[])[1, 2, 3, 4]).Select(i => i * 2);
            Assert.Equal(2, source[0]);

            Assert.Equal(6, source.Skip(2)[0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => source.Skip(4)[0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => source.Skip(14)[0]);

            IReadOnlyList<int> empty = Array.Empty<int>().Select(i => i * 2);
            Assert.Throws<ArgumentOutOfRangeException>(() => empty[0]);
        }

        [Fact]
        public void Select_SourceIsList_First()
        {
            IReadOnlyList<int> source = ((List<int>)[1, 2, 3, 4]).Select(i => i * 2);
            Assert.Equal(2, source[0]);

            Assert.Equal(6, source.Skip(2)[0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => source.Skip(4)[0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => source.Skip(14)[0]);

            IReadOnlyList<int> empty = new List<int>().Select(i => i * 2);
            Assert.Throws<ArgumentOutOfRangeException>(() => empty[0]);
        }

        [Fact]
        public void Select_SourceIsIList_First()
        {
            IReadOnlyList<int> source = ((List<int>)[1, 2, 3, 4]).AsReadOnly().Select(i => i * 2);
            Assert.Equal(2, source[0]);

            Assert.Equal(6, source.Skip(2)[0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => source.Skip(4)[0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => source.Skip(14)[0]);

            IReadOnlyList<int> empty = new List<int>().AsReadOnly().Select(i => i * 2);
            Assert.Throws<ArgumentOutOfRangeException>(() => empty[0]);
        }

        [Fact]
        public void Select_SourceIsArray_Last()
        {
            IReadOnlyList<int> source = ((int[])[1, 2, 3, 4]).Select(i => i * 2);
            Assert.Equal(8, source[^1]);

            Assert.Equal(6, source.Take(3)[^1]);

            IReadOnlyList<int> empty = Array.Empty<int>().Select(i => i * 2);
            Assert.Throws<ArgumentOutOfRangeException>(() => empty[^1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => empty.Skip(1)[^1]);
        }

        [Fact]
        public void Select_SourceIsList_Last()
        {
            IReadOnlyList<int> source = ((List<int>)[1, 2, 3, 4]).Select(i => i * 2);
            Assert.Equal(8, source[^1]);

            Assert.Equal(6, source.Take(3)[^1]);

            IReadOnlyList<int> empty = new List<int>().Select(i => i * 2);
            Assert.Throws<ArgumentOutOfRangeException>(() => empty[^1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => empty.Skip(1)[^1]);
        }

        [Fact]
        public void Select_SourceIsIList_Last()
        {
            IReadOnlyList<int> source = ((List<int>)[1, 2, 3, 4]).AsReadOnly().Select(i => i * 2);
            Assert.Equal(8, source[^1]);

            Assert.Equal(6, source.Take(3)[^1]);

            IReadOnlyList<int> empty = new List<int>().AsReadOnly().Select(i => i * 2);
            Assert.Throws<ArgumentOutOfRangeException>(() => empty[^1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => empty.Skip(1)[^1]);
        }

        [Fact]
        public void Select_SourceIsArray_SkipRepeatCalls()
        {
            IReadOnlyList<int> source = ((int[])[1, 2, 3, 4]).Select(i => i * 2).Skip(1);
            Assert.Equal(source, source);
        }

        [Fact]
        public void Select_SourceIsArraySkipSelect()
        {
            IReadOnlyList<int> source = ((int[])[1, 2, 3, 4]).Select(i => i * 2).Skip(1).Select(i => i + 1);
            Assert.Equal([5, 7, 9], source);
        }

        [Fact]
        public void Select_SourceIsArrayTakeTake()
        {
            IReadOnlyList<int> source = ((int[])[1, 2, 3, 4]).Select(i => i * 2).Take(2).Take(1);
            Assert.Equal([2], source);
            Assert.Equal([2], source.Take(10));
        }

        [Fact]
        public void Select_SourceIsListSkipTakeCount()
        {
            Assert.Equal(3, ((List<int>)[1, 2, 3, 4]).Select(i => i * 2).Take(3).Count);
            Assert.Equal(4, ((List<int>)[1, 2, 3, 4]).Select(i => i * 2).Take(9).Count);
            Assert.Equal(2, ((List<int>)[1, 2, 3, 4]).Select(i => i * 2).Skip(2).Count);
            Assert.Empty(((List<int>)[1, 2, 3, 4]).Select(i => i * 2).Skip(8));
        }

        [Fact]
        public void Select_SourceIsListSkipTake()
        {
            Assert.Equal([2, 4, 6], ((List<int>)[1, 2, 3, 4]).Select(i => i * 2).Take(3));
            Assert.Equal([2, 4, 6, 8], ((List<int>)[1, 2, 3, 4]).Select(i => i * 2).Take(9));
            Assert.Equal([6, 8], ((List<int>)[1, 2, 3, 4]).Select(i => i * 2).Skip(2));
            Assert.Empty(((List<int>)[1, 2, 3, 4]).Select(i => i * 2).Skip(8));
        }

        [Theory]
        [MemberData(nameof(MoveNextAfterDisposeData))]
        public void MoveNextAfterDispose(IReadOnlyList<int> source)
        {
            // Select is specialized for a bunch of different types, so we want
            // to make sure this holds true for all of them.
            List<Func<IReadOnlyList<int>, IReadOnlyList<int>>> identityTransforms =
            [
                e => e,
                e => e.ToArray(),
                e => e.ToList(),
                e => e.Select(i => i) // Multiple Select() chains are optimized
            ];

            foreach (IReadOnlyList<int> equivalentSource in identityTransforms.Select(t => t(source)))
            {
                IReadOnlyList<int> result = equivalentSource.Select(i => i);
                using IEnumerator<int> e = result.GetEnumerator();
                while (e.MoveNext())
                {
                    // Loop until we reach the end of the iterator, @ which pt it gets disposed.
                }

                Assert.False(e.MoveNext()); // MoveNext should not throw an exception after Dispose.
            }
        }

        public static IEnumerable<TheoryDataRow<IReadOnlyList<int>>> MoveNextAfterDisposeData()
        {
            yield return new((int[])[]);
            yield return new(new int[1]);
            yield return new(Indexerable.Range(1, 30));
        }
    }
}
