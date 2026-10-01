using System;
using System.Collections.Generic;
using Xunit;

namespace Indexer.Linq.Tests
{
    public class ZipTests : IndexerableTests
    {
        [Fact]
        public void ImplicitTypeParameters()
        {
            IReadOnlyList<int> first = [1, 2, 3];
            IReadOnlyList<int> second = [2, 5, 9];
            IReadOnlyList<int> expected = [3, 7, 12];

            Assert.Equal(expected, first.Zip(second, (x, y) => x + y));
        }

        [Fact]
        public void ExplicitTypeParameters()
        {
            IReadOnlyList<int> first = [1, 2, 3];
            IReadOnlyList<int> second = [2, 5, 9];
            IReadOnlyList<int> expected = [3, 7, 12];

            Assert.Equal(expected, first.Zip(second, (x, y) => x + y));
        }

        [Fact]
        public void FirstIsNull()
        {
            IReadOnlyList<int> first = null;
            IReadOnlyList<int> second = [2, 5, 9];

            AssertExtensions.Throws<ArgumentNullException>("first", () => first.Zip(second, (x, y) => x + y));
        }

        [Fact]
        public void SecondIsNull()
        {
            IReadOnlyList<int> first = [1, 2, 3];
            IReadOnlyList<int> second = null;

            AssertExtensions.Throws<ArgumentNullException>("second", () => first.Zip<int, int, int>(second, (x, y) => x + y));
        }

        [Fact]
        public void FuncIsNull()
        {
            IReadOnlyList<int> first = [1, 2, 3];
            IReadOnlyList<int> second = [2, 4, 6];
            Func<int, int, int> func = null;

            AssertExtensions.Throws<ArgumentNullException>("resultSelector", () => first.Zip(second, func));
        }

        [Fact]
        public void FirstAndSecondEmpty()
        {
            IReadOnlyList<int> first = [];
            IReadOnlyList<int> second = [];
            static int func(int x, int y) => x + y;
            IReadOnlyList<int> expected = [];

            Assert.Equal(expected, first.Zip(second, func));
        }

        [Fact]
        public void FirstEmptySecondSingle()
        {
            IReadOnlyList<int> first = [];
            IReadOnlyList<int> second = [2];
            static int func(int x, int y) => x + y;
            IReadOnlyList<int> expected = [];

            Assert.Equal(expected, first.Zip(second, func));
        }

        [Fact]
        public void FirstEmptySecondMany()
        {
            IReadOnlyList<int> first = [];
            IReadOnlyList<int> second = [2, 4, 8];
            static int func(int x, int y) => x + y;
            IReadOnlyList<int> expected = [];

            Assert.Equal(expected, first.Zip(second, func));
        }

        [Fact]
        public void SecondEmptyFirstSingle()
        {
            IReadOnlyList<int> first = [1];
            IReadOnlyList<int> second = [];
            static int func(int x, int y) => x + y;
            IReadOnlyList<int> expected = [];

            Assert.Equal(expected, first.Zip(second, func));
        }

        [Fact]
        public void SecondEmptyFirstMany()
        {
            IReadOnlyList<int> first = [1, 2, 3];
            IReadOnlyList<int> second = [];
            static int func(int x, int y) => x + y;
            IReadOnlyList<int> expected = [];

            Assert.Equal(expected, first.Zip(second, func));
        }

        [Fact]
        public void FirstAndSecondSingle()
        {
            IReadOnlyList<int> first = [1];
            IReadOnlyList<int> second = [2];
            static int func(int x, int y) => x + y;
            IReadOnlyList<int> expected = [3];

            Assert.Equal(expected, first.Zip(second, func));
        }

        [Fact]
        public void FirstAndSecondEqualSize()
        {
            IReadOnlyList<int> first = [1, 2, 3];
            IReadOnlyList<int> second = [2, 3, 4];
            static int func(int x, int y) => x + y;
            IReadOnlyList<int> expected = [3, 5, 7];

            Assert.Equal(expected, first.Zip(second, func));
        }

        [Fact]
        public void SecondOneMoreThanFirst()
        {
            IReadOnlyList<int> first = [1, 2];
            IReadOnlyList<int> second = [2, 4, 8];
            static int func(int x, int y) => x + y;
            IReadOnlyList<int> expected = [3, 6];

            Assert.Equal(expected, first.Zip(second, func));
        }

        [Fact]
        public void SecondManyMoreThanFirst()
        {
            IReadOnlyList<int> first = [1, 2];
            IReadOnlyList<int> second = [2, 4, 8, 16];
            static int func(int x, int y) => x + y;
            IReadOnlyList<int> expected = [3, 6];

            Assert.Equal(expected, first.Zip(second, func));
        }

        [Fact]
        public void FirstOneMoreThanSecond()
        {
            IReadOnlyList<int> first = [1, 2, 3];
            IReadOnlyList<int> second = [2, 4];
            static int func(int x, int y) => x + y;
            IReadOnlyList<int> expected = [3, 6];

            Assert.Equal(expected, first.Zip(second, func));
        }

        [Fact]
        public void FirstManyMoreThanSecond()
        {
            IReadOnlyList<int> first = [1, 2, 3, 4];
            IReadOnlyList<int> second = [2, 4];
            static int func(int x, int y) => x + y;
            IReadOnlyList<int> expected = [3, 6];

            Assert.Equal(expected, first.Zip(second, func));
        }

        [Fact]
        public void DelegateFuncChanged()
        {
            IReadOnlyList<int> first = [1, 2, 3, 4];
            IReadOnlyList<int> second = [2, 4, 8];
            Func<int, int, int> func = (x, y) => x + y;
            IReadOnlyList<int> expected = [3, 6, 11];

            Assert.Equal(expected, first.Zip(second, func));

            func = (x, y) => x - y;
            expected = [-1, -2, -5];

            Assert.Equal(expected, first.Zip(second, func));
        }

        [Fact]
        public void LambdaFuncChanged()
        {
            IReadOnlyList<int> first = [1, 2, 3, 4];
            IReadOnlyList<int> second = [2, 4, 8];
            IReadOnlyList<int> expected = [3, 6, 11];

            Assert.Equal(expected, first.Zip(second, (x, y) => x + y));

            expected = [-1, -2, -5];

            Assert.Equal(expected, first.Zip(second, (x, y) => x - y));
        }

        [Fact]
        public void FirstHasFirstElementNull()
        {
            IReadOnlyList<int?> first = [null, 2, 3, 4];
            IReadOnlyList<int> second = [2, 4, 8];
            static int? func(int? x, int y) => x + y;
            IReadOnlyList<int?> expected = [null, 6, 11];

            Assert.Equal(expected, first.Zip(second, func));
        }

        [Fact]
        public void FirstHasLastElementNull()
        {
            IReadOnlyList<int?> first = [1, 2, null];
            IReadOnlyList<int> second = [2, 4, 6, 8];
            static int? func(int? x, int y) => x + y;
            IReadOnlyList<int?> expected = [3, 6, null];

            Assert.Equal(expected, first.Zip(second, func));
        }

        [Fact]
        public void FirstHasMiddleNullValue()
        {
            IReadOnlyList<int?> first = [1, null, 3];
            IReadOnlyList<int> second = [2, 4, 6, 8];
            static int? func(int? x, int y) => x + y;
            IReadOnlyList<int?> expected = [3, null, 9];

            Assert.Equal(expected, first.Zip(second, func));
        }

        [Fact]
        public void FirstAllElementsNull()
        {
            IReadOnlyList<int?> first = [null, null, null];
            IReadOnlyList<int> second = [2, 4, 6, 8];
            static int? func(int? x, int y) => x + y;
            IReadOnlyList<int?> expected = [null, null, null];

            Assert.Equal(expected, first.Zip(second, func));
        }

        [Fact]
        public void SecondHasFirstElementNull()
        {
            IReadOnlyList<int> first = [1, 2, 3, 4];
            IReadOnlyList<int?> second = [null, 4, 6];
            static int? func(int x, int? y) => x + y;
            IReadOnlyList<int?> expected = [null, 6, 9];

            Assert.Equal(expected, first.Zip(second, func));
        }

        [Fact]
        public void SecondHasLastElementNull()
        {
            IReadOnlyList<int> first = [1, 2, 3, 4];
            IReadOnlyList<int?> second = [2, 4, null];
            static int? func(int x, int? y) => x + y;
            IReadOnlyList<int?> expected = [3, 6, null];

            Assert.Equal(expected, first.Zip(second, func));
        }

        [Fact]
        public void SecondHasMiddleElementNull()
        {
            IReadOnlyList<int> first = [1, 2, 3, 4];
            IReadOnlyList<int?> second = [2, null, 6];
            static int? func(int x, int? y) => x + y;
            IReadOnlyList<int?> expected = [3, null, 9];

            Assert.Equal(expected, first.Zip(second, func));
        }

        [Fact]
        public void SecondHasAllElementsNull()
        {
            IReadOnlyList<int> first = [1, 2, 3, 4];
            IReadOnlyList<int?> second = [null, null, null];
            static int? func(int x, int? y) => x + y;
            IReadOnlyList<int?> expected = [null, null, null];

            Assert.Equal(expected, first.Zip(second, func));
        }

        [Fact]
        public void SecondLargerFirstAllNull()
        {
            IReadOnlyList<int?> first = [null, null, null, null];
            IReadOnlyList<int?> second = [null, null, null];
            static int? func(int? x, int? y) => x + y;
            IReadOnlyList<int?> expected = [null, null, null];

            Assert.Equal(expected, first.Zip(second, func));
        }

        [Fact]
        public void FirstSameSizeSecondAllNull()
        {
            IReadOnlyList<int?> first = [null, null, null];
            IReadOnlyList<int?> second = [null, null, null];
            static int? func(int? x, int? y) => x + y;
            IReadOnlyList<int?> expected = [null, null, null];

            Assert.Equal(expected, first.Zip(second, func));
        }

        [Fact]
        public void FirstSmallerSecondAllNull()
        {
            IReadOnlyList<int?> first = [null, null, null];
            IReadOnlyList<int?> second = [null, null, null, null];
            static int? func(int? x, int? y) => x + y;
            IReadOnlyList<int?> expected = [null, null, null];

            Assert.Equal(expected, first.Zip(second, func));
        }

        [Fact]
        public void ForcedToEnumeratorDoesntEnumerate()
        {
            IReadOnlyList<int> iterator = Indexerable.Range(0, 3).Zip(Indexerable.Range(0, 3), (x, y) => x + y);
            // Don't insist on this behaviour, but check it's correct if it happens
            IEnumerator<int> en = iterator as IEnumerator<int>;
            Assert.False(en is not null && en.MoveNext());
        }

        [Fact]
        public void Zip2_ImplicitTypeParameters()
        {
            IReadOnlyList<int> first = [1, 2, 3];
            IReadOnlyList<int> second = [2, 5, 9];
            IReadOnlyList<(int, int)> expected = [(1, 2), (2, 5), (3, 9)];

            Assert.Equal(expected, first.Zip(second));
        }

        [Fact]
        public void Zip2_ExplicitTypeParameters()
        {
            IReadOnlyList<int> first = [1, 2, 3];
            IReadOnlyList<int> second = [2, 5, 9];
            IReadOnlyList<(int, int)> expected = [(1, 2), (2, 5), (3, 9)];

            Assert.Equal(expected, first.Zip<int, int>(second));
        }

        [Fact]
        public void Zip2_FirstIsNull()
        {
            IReadOnlyList<int> first = null;
            IReadOnlyList<int> second = [2, 5, 9];

            AssertExtensions.Throws<ArgumentNullException>("first", () => first.Zip<int, int>(second));
        }

        [Fact]
        public void Zip2_SecondIsNull()
        {
            IReadOnlyList<int> first = [1, 2, 3];
            IReadOnlyList<int> second = null;

            AssertExtensions.Throws<ArgumentNullException>("second", () => first.Zip<int, int>(second));
        }

        [Fact]
        public void Zip2_FirstAndSecondEmpty()
        {
            IReadOnlyList<int> first = [];
            IReadOnlyList<int> second = [];
            IReadOnlyList<(int, int)> expected = [];

            Assert.Equal(expected, first.Zip(second));
        }

        [Fact]
        public void Zip2_FirstEmptySecondSingle()
        {
            IReadOnlyList<int> first = [];
            IReadOnlyList<int> second = [2];
            IReadOnlyList<(int, int)> expected = [];

            Assert.Equal(expected, first.Zip(second));
        }

        [Fact]
        public void Zip2_FirstEmptySecondMany()
        {
            IReadOnlyList<int> first = [];
            IReadOnlyList<int> second = [2, 4, 8];
            IReadOnlyList<(int, int)> expected = [];

            Assert.Equal(expected, first.Zip(second));
        }

        [Fact]
        public void Zip2_SecondEmptyFirstSingle()
        {
            IReadOnlyList<int> first = [1];
            IReadOnlyList<int> second = [];
            IReadOnlyList<(int, int)> expected = [];

            Assert.Equal(expected, first.Zip(second));
        }

        [Fact]
        public void Zip2_SecondEmptyFirstMany()
        {
            IReadOnlyList<int> first = [1, 2, 3];
            IReadOnlyList<int> second = [];
            IReadOnlyList<(int, int)> expected = [];

            Assert.Equal(expected, first.Zip(second));
        }

        [Fact]
        public void Zip2_FirstAndSecondSingle()
        {
            IReadOnlyList<int> first = [1];
            IReadOnlyList<int> second = [2];
            IReadOnlyList<(int, int)> expected = [(1, 2)];

            Assert.Equal(expected, first.Zip(second));
        }

        [Fact]
        public void Zip2_FirstAndSecondEqualSize()
        {
            IReadOnlyList<int> first = [1, 2, 3];
            IReadOnlyList<int> second = [2, 3, 4];
            IReadOnlyList<(int, int)> expected = [(1, 2), (2, 3), (3, 4)];

            Assert.Equal(expected, first.Zip(second));
        }

        [Fact]
        public void Zip2_SecondOneMoreThanFirst()
        {
            IReadOnlyList<int> first = [1, 2];
            IReadOnlyList<int> second = [2, 4, 8];
            IReadOnlyList<(int, int)> expected = [(1, 2), (2, 4)];

            Assert.Equal(expected, first.Zip(second));
        }

        [Fact]
        public void Zip2_SecondManyMoreThanFirst()
        {
            IReadOnlyList<int> first = [1, 2];
            IReadOnlyList<int> second = [2, 4, 8, 16];
            IReadOnlyList<(int, int)> expected = [(1, 2), (2, 4)];

            Assert.Equal(expected, first.Zip(second));
        }

        [Fact]
        public void Zip2_FirstOneMoreThanSecond()
        {
            IReadOnlyList<int> first = [1, 2, 3];
            IReadOnlyList<int> second = [2, 4];
            IReadOnlyList<(int, int)> expected = [(1, 2), (2, 4)];

            Assert.Equal(expected, first.Zip(second));
        }

        [Fact]
        public void Zip2_FirstManyMoreThanSecond()
        {
            IReadOnlyList<int> first = [1, 2, 3, 4];
            IReadOnlyList<int> second = [2, 4];
            IReadOnlyList<(int, int)> expected = [(1, 2), (2, 4)];

            Assert.Equal(expected, first.Zip(second));
        }

        [Fact]
        public void Zip2_NestedTuple()
        {
            IReadOnlyList<int> first = [1, 3, 5];
            IReadOnlyList<int> second = [2, 4, 6];
            IReadOnlyList<(int, int)> third = [(1, 2), (3, 4), (5, 6)];

            Assert.Equal(third, first.Zip(second));

            IReadOnlyList<string> fourth = ["one", "two", "three"];

            IReadOnlyList<((int, int), string)> final = [((1, 2), "one"), ((3, 4), "two"), ((5, 6), "three")];
            Assert.Equal(final, third.Zip(fourth));
        }

        [Fact]
        public void Zip2_TupleNames()
        {
            (int First, int Second) = ((int[])[1, 2, 3]).Zip([2, 4, 6])[0];
            Assert.Equal(First, First);
            Assert.Equal(Second, Second);
        }

        [Fact]
        public void Zip3_FirstIsNull()
        {
            IReadOnlyList<int> first = null;
            IReadOnlyList<int> second = [4, 5, 6];
            IReadOnlyList<int> third = [7, 8, 9];

            AssertExtensions.Throws<ArgumentNullException>("first", () => first.Zip(second, third));
        }

        [Fact]
        public void Zip3_SecondIsNull()
        {
            IReadOnlyList<int> first = [1, 2, 3];
            IReadOnlyList<int> second = null;
            IReadOnlyList<int> third = [4, 5, 6];

            AssertExtensions.Throws<ArgumentNullException>("second", () => first.Zip(second, third));
        }

        [Fact]
        public void Zip3_ThirdIsNull()
        {
            IReadOnlyList<int> first = [1, 2, 3];
            IReadOnlyList<int> second = [4, 5, 6];
            IReadOnlyList<int> third = null;

            AssertExtensions.Throws<ArgumentNullException>("third", () => first.Zip(second, third));
        }

        [Fact]
        public void Zip3_ThirdEmpty()
        {
            IReadOnlyList<int> first = [1, 2, 3];
            IReadOnlyList<int> second = [4, 5, 6];
            IReadOnlyList<int> third = [];
            IReadOnlyList<(int, int, int)> expected = [];

            Assert.Equal(expected, first.Zip(second, third));
        }

        [Fact]
        public void Zip3_ImplicitTypeParameters()
        {
            IReadOnlyList<int> first = [1, 2];
            IReadOnlyList<int> second = [3, 4];
            IReadOnlyList<int> third = [5, 6];
            IReadOnlyList<(int, int, int)> expected = [(1, 3, 5), (2, 4, 6)];

            Assert.Equal(expected, first.Zip(second, third));
        }

        [Fact]
        public void Zip3_ExplicitTypeParameters()
        {
            IReadOnlyList<int> first = [1, 2];
            IReadOnlyList<int> second = [3, 4];
            IReadOnlyList<int> third = [5, 6];
            IReadOnlyList<(int, int, int)> expected = [(1, 3, 5), (2, 4, 6)];

            Assert.Equal(expected, first.Zip<int, int, int>(second, third));
        }

        [Fact]
        public void Zip3_ThirdOneMore()
        {
            IReadOnlyList<int> first = [1, 2];
            IReadOnlyList<int> second = [3, 4];
            IReadOnlyList<int> third = [5, 6, 7];
            IReadOnlyList<(int, int, int)> expected = [(1, 3, 5), (2, 4, 6)];

            Assert.Equal(expected, first.Zip(second, third));
        }

        [Fact]
        public void Zip3_ThirdManyMore()
        {
            IReadOnlyList<int> first = [1, 2];
            IReadOnlyList<int> second = [3, 4];
            IReadOnlyList<int> third = [5, 6, 7, 8];
            IReadOnlyList<(int, int, int)> expected = [(1, 3, 5), (2, 4, 6)];

            Assert.Equal(expected, first.Zip(second, third));
        }

        [Fact]
        public void Zip3_ThirdOneLess()
        {
            IReadOnlyList<int> first = [1, 2];
            IReadOnlyList<int> second = [3, 4];
            IReadOnlyList<int> third = [5];
            IReadOnlyList<(int, int, int)> expected = [(1, 3, 5)];

            Assert.Equal(expected, first.Zip(second, third));
        }

        [Fact]
        public void Zip3_ThirdManyLess()
        {
            IReadOnlyList<int> first = [1, 2, 3];
            IReadOnlyList<int> second = [3, 4, 5];
            IReadOnlyList<int> third = [5];
            IReadOnlyList<(int, int, int)> expected = [(1, 3, 5)];

            Assert.Equal(expected, first.Zip(second, third));
        }
    }
}
