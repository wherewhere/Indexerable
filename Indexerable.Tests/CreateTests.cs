using System;
using System.Collections.Generic;
using Xunit;

namespace Indexer.Linq.Tests
{
    public class CreateTests
    {
        [Fact]
        public void Create_ProducesValuesByZeroBasedIndexLazily()
        {
            List<int> selectedIndices = [];
            IReadOnlyList<int> result = Indexerable.Create(4, index =>
            {
                selectedIndices.Add(index);
                return index * 10;
            });

            Assert.Equal(4, result.Count);
            Assert.Empty(selectedIndices);

            Assert.Equal(20, result[2]);
            Assert.Equal([2], selectedIndices);

            Assert.Equal(20, result[2]);
            Assert.Equal([2, 2], selectedIndices);
            Assert.Equal(0, result[0]);
        }

        [Fact]
        public void Create_ZeroCountReturnsEmptyWithoutCallingSelector()
        {
            bool selectorCalled = false;

            IReadOnlyList<int> result = Indexerable.Create(0, _ =>
            {
                selectorCalled = true;
                return 1;
            });

            Assert.Empty(result);
            Assert.False(selectorCalled);
        }

        [Fact]
        public void Create_OneCountContainsOnlyIndexZero()
        {
            IReadOnlyList<int> result = Indexerable.Create(1, index => index + 42);

            Assert.Single(result);
            Assert.Equal([42], result);
            Assert.Equal(42, result[0]);
        }

        [Fact]
        public void Create_NegativeCountThrowsArgumentOutOfRangeException()
        {
            ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(
                () => Indexerable.Create(-1, index => index));

            Assert.Equal("count", exception.ParamName);
        }

        [Fact]
        public void Create_NegativeCountIsValidatedBeforeNullSelector()
        {
            ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(
                () => Indexerable.Create<int>(-1, null));

            Assert.Equal("count", exception.ParamName);
        }

        [Fact]
        public void Create_NullSelectorThrowsArgumentNullExceptionEvenForEmptyCount()
        {
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(
                () => Indexerable.Create<int>(0, null));

            Assert.Equal("selector", exception.ParamName);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(3)]
        public void Indexer_OutOfBoundsThrowsArgumentOutOfRangeException(int index)
        {
            IReadOnlyList<int> result = Indexerable.Create(3, value => value * 2);

            ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(
                () => _ = result[index]);

            Assert.Equal("index", exception.ParamName);
        }

        [Fact]
        public void Indexer_EmptyListRejectsZero()
        {
            IReadOnlyList<int> result = Indexerable.Create(0, value => value);

            ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(
                () => _ = result[0]);

            Assert.Equal("index", exception.ParamName);
        }

        [Fact]
        public void Create_EnumerationSelectsValuesInOrder()
        {
            IReadOnlyList<int> result = Indexerable.Create(4, index => index + 1);

            Assert.Equal([1, 2, 3, 4], result);
        }

        [Fact]
        public void Create_IndexedSelectUsesZeroBasedIndices()
        {
            IReadOnlyList<string> result = Indexerable.Create(3, index => index * 10)
                .Select((value, index) => $"{value}:{index}");

            Assert.Equal(["0:0", "10:1", "20:2"], result);
        }

        [Fact]
        public void CreateOffsetIndexer_IndexedSelectUsesZeroBasedIndices()
        {
            IReadOnlyList<string> result = Indexerable.Create(5, index => index * 10)
                .Skip(2)
                .Take(2)
                .Select((value, index) => $"{value}:{index}");

            Assert.Equal(["20:0", "30:1"], result);
        }

        [Fact]
        public void Create_ContainsAndIndexOfUseSelectedValues()
        {
            IReadOnlyList<string> result = Indexerable.Create(4, index => $"value-{index}");

            Assert.Contains("value-2", result);
            Assert.DoesNotContain("missing", result);
            Assert.Equal(2, result.IndexOf("value-2"));
            Assert.Equal(-1, result.IndexOf("missing"));
        }

        [Fact]
        public void Create_ContainsAndIndexOfSupportNullValues()
        {
            IReadOnlyList<string> result = Indexerable.Create(3, index => index == 1 ? null : $"value-{index}");

            Assert.Contains(null, result);
            Assert.Equal(1, result.IndexOf((string)null));
            Assert.DoesNotContain("missing", result);
            Assert.Equal(-1, result.IndexOf("missing"));
        }

        [Fact]
        public void Create_CopyToCopiesValuesAtRequestedArrayOffset()
        {
            IReadOnlyList<int> result = Indexerable.Create(3, index => index * 10);
            int[] destination = [-1, -1, -1, -1, -1];

            ((ICollection<int>)result).CopyTo(destination, 1);

            Assert.Equal([-1, 0, 10, 20, -1], destination);
        }

        [Fact]
        public void Create_SkipHonorsNonPositiveAndBoundaryCounts()
        {
            IReadOnlyList<int> result = Indexerable.Create(5, index => index);

            Assert.Equal([0, 1, 2, 3, 4], result.Skip(0));
            Assert.Equal([0, 1, 2, 3, 4], result.Skip(-2));
            Assert.Equal([2, 3, 4], result.Skip(2));
            Assert.Empty(result.Skip(5));
            Assert.Empty(result.Skip(6));
        }

        [Fact]
        public void Create_SkipLastHonorsNonPositiveAndBoundaryCounts()
        {
            IReadOnlyList<int> result = Indexerable.Create(5, index => index);

            Assert.Equal([0, 1, 2, 3, 4], result.SkipLast(0));
            Assert.Equal([0, 1, 2, 3, 4], result.SkipLast(-2));
            Assert.Equal([0, 1, 2], result.SkipLast(2));
            Assert.Empty(result.SkipLast(5));
            Assert.Empty(result.SkipLast(6));
        }

        [Fact]
        public void Create_TakeHonorsNonPositiveAndBoundaryCounts()
        {
            IReadOnlyList<int> result = Indexerable.Create(5, index => index);

            Assert.Empty(result.Take(0));
            Assert.Empty(result.Take(-2));
            Assert.Equal([0, 1, 2], result.Take(3));
            Assert.Equal([0, 1, 2, 3, 4], result.Take(5));
            Assert.Equal([0, 1, 2, 3, 4], result.Take(6));
        }

        [Fact]
        public void Create_TakeLastHonorsNonPositiveAndBoundaryCounts()
        {
            IReadOnlyList<int> result = Indexerable.Create(5, index => index);

            Assert.Empty(result.TakeLast(0));
            Assert.Empty(result.TakeLast(-2));
            Assert.Equal([2, 3, 4], result.TakeLast(3));
            Assert.Equal([0, 1, 2, 3, 4], result.TakeLast(5));
            Assert.Equal([0, 1, 2, 3, 4], result.TakeLast(6));
        }

        [Fact]
        public void Create_CombinationsOfSkipAndTakeKeepCorrectOffsets()
        {
            IReadOnlyList<int> result = Indexerable.Create(8, index => index * 10);

            Assert.Equal([30, 40, 50], result.Skip(3).Take(3));
            Assert.Equal([30, 40, 50], result.Skip(2).Skip(1).Take(3));
            Assert.Equal([60, 70], result.Skip(1).TakeLast(2));
            Assert.Equal([50, 60], result.TakeLast(5).Skip(2).Take(2));
        }

        [Fact]
        public void Create_CombinationsOfSkipLastAndTakeLastKeepCorrectOffsets()
        {
            IReadOnlyList<int> result = Indexerable.Create(8, index => index * 10);

            Assert.Equal([0, 10, 20, 30, 40], result.SkipLast(3).Take(5));
            Assert.Equal([20, 30, 40], result.Skip(2).SkipLast(3).TakeLast(3));
            Assert.Equal([50, 60], result.SkipLast(1).TakeLast(2));
            Assert.Equal([40, 50], result.TakeLast(5).SkipLast(2).TakeLast(2));
        }

        [Fact]
        public void Create_ChainedTakeAndSkipLastRespectEmptyAndNonPositiveCounts()
        {
            IReadOnlyList<int> result = Indexerable.Create(4, index => index);

            Assert.Empty(result.Skip(-1).Take(0).SkipLast(-1));
            Assert.Equal([0, 1, 2, 3], result.Skip(0).Take(10).SkipLast(0));
            Assert.Equal([1, 2], result.Skip(1).Take(3).SkipLast(1));
        }

        [Fact]
        public void Create_OffsetIndexerSupportsIndexerCountAndEnumeration()
        {
            IReadOnlyList<int> result = Indexerable.Create(6, index => index * 10).Skip(2).Take(3);

            Assert.Equal(3, result.Count);
            Assert.Equal(20, result[0]);
            Assert.Equal(40, result[2]);
            Assert.Equal([20, 30, 40], result);

            foreach (int index in new[] { -1, 3 })
            {
                ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(
                    () => _ = result[index]);

                Assert.Equal("index", exception.ParamName);
            }
        }

        [Fact]
        public void Create_OffsetIndexerContainsIndexOfAndCopyToUseOffsetValues()
        {
            IReadOnlyList<int> result = Indexerable.Create(6, index => index * 10).Skip(2).Take(3);
            int[] destination = [-1, -1, -1, -1, -1];

            Assert.Contains(30, result);
            Assert.DoesNotContain(10, result);
            Assert.Equal(1, result.IndexOf(30));
            Assert.Equal(-1, result.IndexOf(10));

            ((ICollection<int>)result).CopyTo(destination, 1);

            Assert.Equal([-1, 20, 30, 40, -1], destination);
        }

        [Fact]
        public void Create_OffsetIndexerSkipAndTakeOperatorsCoverTheirBranches()
        {
            IReadOnlyList<int> result = Indexerable.Create(6, index => index * 10).Skip(2);

            Assert.Same(result, result.Skip(0));
            Assert.Same(result, result.Skip(-1));
            Assert.Equal([40, 50], result.Skip(2));
            Assert.Empty(result.Skip(4));
            Assert.Empty(result.Skip(5));

            Assert.Same(result, result.SkipLast(0));
            Assert.Same(result, result.SkipLast(-1));
            Assert.Equal([20, 30], result.SkipLast(2));
            Assert.Empty(result.SkipLast(4));
            Assert.Empty(result.SkipLast(5));

            Assert.Empty(result.Take(0));
            Assert.Empty(result.Take(-1));
            Assert.Equal([20, 30], result.Take(2));
            Assert.Same(result, result.Take(4));
            Assert.Same(result, result.Take(5));

            Assert.Empty(result.TakeLast(0));
            Assert.Empty(result.TakeLast(-1));
            Assert.Equal([40, 50], result.TakeLast(2));
            Assert.Same(result, result.TakeLast(4));
            Assert.Same(result, result.TakeLast(5));
        }

        [Fact]
        public void Create_ChainedOffsetIndexerOperatorsKeepAccumulatedOffsets()
        {
            IReadOnlyList<int> result = Indexerable.Create(8, index => index * 10)
                .Skip(1)
                .Skip(2)
                .SkipLast(1)
                .Take(3)
                .TakeLast(2);

            Assert.Equal([40, 50], result);
        }
    }
}
