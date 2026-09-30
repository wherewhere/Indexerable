using System;
using System.Collections.Generic;
using Xunit;

namespace Indexer.Linq.Tests
{
    public class ConcatTests : IndexerableTests
    {
        [Theory]
        [InlineData(new[] { 2, 3, 2, 4, 5 }, new[] { 1, 9, 4 })]
        public void SameResultsWithQueryAndRepeatCalls_Int(IReadOnlyList<int> first, IReadOnlyList<int> second)
        {
            // workaround: xUnit type inference doesn't work if the input type is not T (like IReadOnlyList<T>)
            SameResultsWithQueryAndRepeatCallsWorker(first, second);
        }

        [Theory]
        [InlineData(new[] { "AAA", "", "q", "C", "#", "!@#$%^", "0987654321", "Calling Twice" }, new[] { "!@#$%^", "C", "AAA", "", "Calling Twice", "SoS" })]
        public void SameResultsWithQueryAndRepeatCalls_String(IReadOnlyList<string> first, IReadOnlyList<string> second)
        {
            // workaround: xUnit type inference doesn't work if the input type is not T (like IReadOnlyList<T>)
            SameResultsWithQueryAndRepeatCallsWorker(first, second);
        }

        private static void SameResultsWithQueryAndRepeatCallsWorker<T>(IReadOnlyList<T> first, IReadOnlyList<T> second)
        {
            first = from item in first select item;
            second = from item in second select item;

            Assert.Equal(first.Concat(second), first.Concat(second));
            Assert.Equal(second.Concat(first), second.Concat(first));
        }

        [Theory]
        [InlineData(new int[] { }, new int[] { }, new int[] { })] // Both inputs are empty
        [InlineData(new int[] { }, new int[] { 2, 6, 4, 6, 2 }, new int[] { 2, 6, 4, 6, 2 })] // One is empty
        [InlineData(new int[] { 2, 3, 5, 9 }, new int[] { 8, 10 }, new int[] { 2, 3, 5, 9, 8, 10 })] // Neither side is empty
        public void PossiblyEmptyInputs(IReadOnlyList<int> first, IReadOnlyList<int> second, IReadOnlyList<int> expected)
        {
            Assert.Equal(expected, first.Concat(second));
            Assert.Equal(expected.Skip(first.Count).Concat(expected.Take(first.Count)), second.Concat(first)); // Swap the inputs around
        }

        [Fact]
        public void ForcedToEnumeratorDoesntEnumerate()
        {
            IReadOnlyList<int> iterator = Indexerable.Range(0, 3).Concat(Indexerable.Range(0, 3));
            // Don't insist on this behaviour, but check it's correct if it happens
            IEnumerator<int> en = iterator as IEnumerator<int>;
            Assert.False(en is not null && en.MoveNext());
        }

        [Fact]
        public void FirstNull()
        {
            AssertExtensions.Throws<ArgumentNullException>("first", () => ((IReadOnlyList<int>)null).Concat(Indexerable.Range(0, 0)));
            AssertExtensions.Throws<ArgumentNullException>("first", () => ((IReadOnlyList<int>)null).Concat(null)); // If both inputs are null, throw for "first" first
        }

        [Fact]
        public void SecondNull()
        {
            AssertExtensions.Throws<ArgumentNullException>("second", () => Indexerable.Range(0, 0).Concat(null));
        }

        [Theory]
        [MemberData(nameof(ArraySourcesData))]
        [MemberData(nameof(SelectArraySourcesData))]
        [MemberData(nameof(EnumerableSourcesData))]
        [MemberData(nameof(ListSourcesData))]
        [MemberData(nameof(ConcatOfConcatsData))]
        [MemberData(nameof(ConcatWithSelfData))]
        [MemberData(nameof(ChainedCollectionConcatData))]
        [MemberData(nameof(AppendedPrependedConcatAlternationsData))]
        public void VerifyEquals(IReadOnlyList<int> expected, IReadOnlyList<int> actual)
        {
            // workaround: xUnit type inference doesn't work if the input type is not T (like IEnumerable<T>)
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(ArraySourcesData))]
        [MemberData(nameof(SelectArraySourcesData))]
        [MemberData(nameof(EnumerableSourcesData))]
        [MemberData(nameof(ListSourcesData))]
        [MemberData(nameof(ConcatOfConcatsData))]
        [MemberData(nameof(ConcatWithSelfData))]
        [MemberData(nameof(ChainedCollectionConcatData))]
        [MemberData(nameof(AppendedPrependedConcatAlternationsData))]
        [MemberData(nameof(ConcatWithEmptyEnumerableData))]
        public void First_Last_ElementAt(IReadOnlyList<int> _, IReadOnlyList<int> actual)
        {
            int count = actual.Count;
            if (count == 0)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => actual[0]);
                Assert.Throws<ArgumentOutOfRangeException>(() => actual[^1]);
            }
            else
            {
                int first = actual[0];
                int last = actual[^1];
                int elementAt = actual[count / 2];

                int enumeratedFirst = 0, enumeratedLast = 0, enumeratedElementAt = 0;
                int i = 0;
                foreach (int item in actual)
                {
                    if (i == 0)
                    {
                        enumeratedFirst = item;
                    }

                    if (i == count / 2)
                    {
                        enumeratedElementAt = item;
                    }

                    enumeratedLast = item;
                    i++;
                }

                Assert.Equal(enumeratedFirst, first);
                Assert.Equal(enumeratedLast, last);
                Assert.Equal(enumeratedElementAt, elementAt);
            }
        }

        public static IEnumerable<TheoryDataRow<IReadOnlyList<int>, IReadOnlyList<int>>> ArraySourcesData() => GenerateSourcesData(outerTransform: e => (int[])[.. e]);

        public static IEnumerable<TheoryDataRow<IReadOnlyList<int>, IReadOnlyList<int>>> SelectArraySourcesData() => GenerateSourcesData(outerTransform: e => (int[])[.. e.Select(i => i)]);

        public static IEnumerable<TheoryDataRow<IReadOnlyList<int>, IReadOnlyList<int>>> EnumerableSourcesData() => GenerateSourcesData();

        public static IEnumerable<TheoryDataRow<IReadOnlyList<int>, IReadOnlyList<int>>> ListSourcesData() => GenerateSourcesData(outerTransform: e => (List<int>)[.. e]);

        public static IEnumerable<TheoryDataRow<IReadOnlyList<int>, IReadOnlyList<int>>> ConcatOfConcatsData()
        {
            yield return new(
                Indexerable.Range(0, 20),
                Indexerable.Concat(
                    Indexerable.Concat(
                        Indexerable.Range(0, 4),
                        Indexerable.Range(4, 6)),
                    Indexerable.Concat(
                        Indexerable.Range(10, 3),
                        Indexerable.Range(13, 7)))
            );
        }

        public static IEnumerable<TheoryDataRow<IReadOnlyList<int>, IReadOnlyList<int>>> ConcatWithSelfData()
        {
            IReadOnlyList<int> source = Indexerable.Repeat(1, 4).Concat(Indexerable.Repeat(1, 5));
            source = source.Concat(source);

            yield return new(Indexerable.Repeat(1, 18), source);
        }

        public static IEnumerable<TheoryDataRow<IReadOnlyList<int>, IReadOnlyList<int>>> ChainedCollectionConcatData() => GenerateSourcesData(innerTransform: e => (List<int>)[.. e]);

        public static IEnumerable<TheoryDataRow<IReadOnlyList<int>, IReadOnlyList<int>>> AppendedPrependedConcatAlternationsData()
        {
            const int EnumerableCount = 4; // How many enumerables to concat together per test case.

            int[] foundation = [];
            List<int> expected = [];
            IReadOnlyList<int> actual = foundation;

            // each bit in the last EnumerableCount bits of i represent whether we want to prepend/append a sequence for this iteration.
            // if it's set, we'll prepend. otherwise, we'll append.
            for (int i = 0; i < (1 << EnumerableCount); i++)
            {
                // each bit in last EnumerableCount bits of j is set if we want to ensure the nth enumerable
                // concat'd is an ICollection.
                // Note: It is important we run over the all-bits-set case, since currently
                // Concat is specialized for when all inputs are ICollection.
                for (int j = 0; j < (1 << EnumerableCount); j++)
                {
                    for (int k = 0; k < EnumerableCount; k++) // k is how much bits we shift by, and also the item that gets appended/prepended.
                    {
                        IReadOnlyList<int> nextRange = Indexerable.Range(k, 1);
                        bool prepend = ((i >> k) & 1) != 0;
                        bool forceCollection = ((j >> k) & 1) != 0;

                        if (forceCollection)
                        {
                            nextRange = (List<int>)[.. nextRange];
                        }

                        actual = prepend ? nextRange.Concat(actual) : actual.Concat(nextRange);
                        if (prepend)
                        {
                            expected.Insert(0, k);
                        }
                        else
                        {
                            expected.Add(k);
                        }
                    }

                    yield return new((int[])[.. expected], (int[])[.. actual]);

                    actual = foundation;
                    expected.Clear();
                }
            }
        }

        public static IEnumerable<TheoryDataRow<IReadOnlyList<int>, IReadOnlyList<int>>> ConcatWithEmptyEnumerableData()
        {
            List<int> baseList = [0, 1, 2, 3, 4];

            yield return
            new(
                Indexerable.Range(0, 5),
                Indexerable.Concat(Indexerable.Concat((List<int>)[], (List<int>)[]), baseList)
            );
            yield return
            new(
                Indexerable.Range(0, 5),
                Indexerable.Concat((List<int>)[], baseList)
            );
            yield return
            new(
                Indexerable.Range(0, 5),
                Indexerable.Concat(Indexerable.Concat(baseList, (List<int>)[]), (List<int>)[])
            );
            yield return
            new(
                Indexerable.Range(0, 5),
                Indexerable.Concat(baseList, (List<int>)[])
            );
        }

        private static IEnumerable<TheoryDataRow<IReadOnlyList<int>, IReadOnlyList<int>>> GenerateSourcesData(
            Func<IReadOnlyList<int>, IReadOnlyList<int>> outerTransform = null,
            Func<IReadOnlyList<int>, IReadOnlyList<int>> innerTransform = null)
        {
            outerTransform ??= (e => e);
            innerTransform ??= (e => e);

            for (int i = 0; i <= 6; i++)
            {
                IReadOnlyList<int> expected = Indexerable.Range(0, i * 3);
                IReadOnlyList<int> actual = [];
                for (int j = 0; j < i; j++)
                {
                    actual = outerTransform(actual.Concat(innerTransform(Indexerable.Range(j * 3, 3))));
                }

                yield return new(expected, actual);
            }
        }
    }
}
