using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Indexer.Linq.Tests
{
    public class EmptyIndexerableTests : IndexerableTests
    {
        [SuppressMessage("Style", "IDE0301:简化集合初始化", Justification = "<挂起>")]
        private static void TestEmptyCached<T>()
        {
            IReadOnlyList<T> enumerable1 = Indexerable.Empty<T>();
            IReadOnlyList<T> enumerable2 = Indexerable.Empty<T>();

            Assert.Same(enumerable1, enumerable2);
        }

        [Fact]
        public void EmptyEnumerableCachedTest()
        {
            TestEmptyCached<int>();
            TestEmptyCached<string>();
            TestEmptyCached<object>();
            TestEmptyCached<EmptyIndexerableTests>();
        }

        private static void TestEmptyEmpty<T>()
        {
            Assert.Equal((T[])[], Indexerable.Empty<T>());
            Assert.Empty(Indexerable.Empty<T>());
            Assert.Same(Indexerable.Empty<T>().GetEnumerator(), Indexerable.Empty<T>().GetEnumerator());
        }

        [Fact]
        public void EmptyEnumerableIsIndeedEmpty()
        {
            TestEmptyEmpty<int>();
            TestEmptyEmpty<string>();
            TestEmptyEmpty<object>();
            TestEmptyEmpty<EmptyIndexerableTests>();
        }

        [Fact]
        public void IListImplementationIsValid()
        {
            IList<int> list = Assert.IsType<IList<int>>(Indexerable.Empty<int>(), exactMatch: false);
            IReadOnlyList<int> roList = Assert.IsType<IReadOnlyList<int>>(Indexerable.Empty<int>(), exactMatch: false);

            Assert.Throws<NotSupportedException>(() => list.Add(42));
            Assert.Throws<NotSupportedException>(() => list.Insert(0, 42));
            Assert.Throws<NotSupportedException>(list.Clear);
            Assert.Throws<NotSupportedException>(() => list.Remove(42));
            Assert.Throws<NotSupportedException>(() => list.RemoveAt(0));
            Assert.Throws<NotSupportedException>(() => list[0] = 42);
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => _ = list[0]);
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => _ = roList[0]);

            Assert.True(list.IsReadOnly);
            Assert.Empty(list);
            Assert.Empty(roList);

            Assert.False(list.Contains(42));
            Assert.Equal(-1, list.IndexOf(42));

            list.CopyTo([], 0);
            AssertExtensions.Throws<ArgumentException>("array", () => list.CopyTo([], 1));
            int[] array = [42];
            list.CopyTo(array, 0);
            Assert.Equal(42, array[0]);
        }
    }
}
