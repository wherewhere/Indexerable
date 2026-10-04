using System;
using System.Collections.Generic;
using Xunit;

namespace Indexer.Linq.Tests
{
    public class AsEnumerableTests
    {
        [Fact]
        public void SameResultsRepeatCallsIntQuery()
        {
            IReadOnlyList<int> q = from x in new[] { 9999, 0, 888, -1, 66, -777, 1, 2, -12345 }
                                   select x;

            Assert.Equal(q.AsReadOnlyList(), q.AsReadOnlyList());
        }

        [Fact]
        public void SameResultsRepeatCallsStringQuery()
        {
            IReadOnlyList<string> q = from x in new[] { "!@#$%^", "C", "AAA", "", "Calling Twice", "SoS", string.Empty }
                                      select x;

            Assert.Equal(q.AsReadOnlyList(), q.AsReadOnlyList());
        }

        [Fact]
        public void NullSourceAllowed()
        {
            int[] source = null;

            Assert.Null(source.AsReadOnlyList());
            AssertExtensions.Throws<ArgumentNullException>("source", () => source.ToReadOnlyList());
        }

        [Fact]
        public void OneElement()
        {
            int[] source = [2];

            Assert.Equal(source, source.AsReadOnlyList());
            Assert.Equal(source, source.ToReadOnlyList());
        }

        [Fact]
        public void SomeElements()
        {
            int?[] source = [-5, 0, 1, -4, 3, null, 10];

            Assert.Equal(source, source.AsReadOnlyList());
            Assert.Equal(source, source.ToReadOnlyList());
        }

        [Fact]
        public void IListNullSource()
        {
            IList<int> source = null;

            AssertExtensions.Throws<ArgumentNullException>("source", () => source.ToReadOnlyList());
        }

        [Fact]
        public void IListOneElement()
        {
            IList<int> source = [2];

            Assert.Equal(source, source.ToReadOnlyList());
        }

        [Fact]
        public void IListSomeElements()
        {
            IList<int?> source = [-5, 0, 1, -4, 3, null, 10];

            Assert.Equal(source, source.ToReadOnlyList());
        }

        [Fact]
        public void StringNullSource()
        {
            string source = null;

            AssertExtensions.Throws<ArgumentNullException>("source", () => source.ToReadOnlyList());
        }

        [Fact]
        public void String()
        {
            string source = "Hello, World!";

            Assert.Equal([.. source], source.ToReadOnlyList());
            Assert.Equal(source, source.ToReadOnlyList().ToString());
        }

        [Fact]
        public void NonGenericIListFindsNullElements()
        {
            System.Collections.IList source = (System.Collections.IList)Indexerable.Repeat<string>(null!, 2);

            Assert.True(source.Contains(null));
            Assert.Equal(0, source.IndexOf(null));
            Assert.False(source.Contains(1));
            Assert.Equal(-1, source.IndexOf(1));
        }
    }
}
