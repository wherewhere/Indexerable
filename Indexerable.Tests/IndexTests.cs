using System;
using System.Collections.Generic;
using Xunit;

namespace Indexer.Linq.Tests
{
    public class IndexTests : IndexerableTests
    {
        [Fact]
        public void Empty()
        {
            Assert.Empty(Indexerable.Empty<int>().Index());
        }

        [Fact]
        public void Index_SourceIsNull_ArgumentNullExceptionThrown()
        {
            IReadOnlyList<int> source = null;

            AssertExtensions.Throws<ArgumentNullException>("source", () => source.Index());
        }

        [Fact]
        public void Index()
        {
            string[] source = ["a", "b"];
            (int Index, string Item)[] actual = [.. source.Index()];
            (int Index, string Item)[] expected = [(0, "a"), (1, "b")];
            AssertExtensions.SequenceEqual(expected, actual);
        }
    }
}
