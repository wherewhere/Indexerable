using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Indexer.Linq.Tests
{
    public class CastTests : IndexerableTests
    {
        [Fact]
        public void CastIntToLongThrows()
        {
            int[] q = [9999, 0, 888, -1, 66, -777, 1, 2, -12345];

            IReadOnlyList<long> rst = q.Cast<long>();
            Assert.Throws<InvalidCastException>(() => { foreach (long t in rst) {; } });

            rst = q.Cast<int, long>();
            Assert.Throws<InvalidCastException>(() => { foreach (long t in rst) {; } });
        }

        [Fact]
        public void CastByteToUShortThrows()
        {
            byte[] q = [0, 255, 127, 128, 1, 33, 99];

            IReadOnlyList<ushort> rst = q.Cast<ushort>();
            Assert.Throws<InvalidCastException>(() => { foreach (ushort t in rst) {; } });

            rst = q.Cast<byte, ushort>();
            Assert.Throws<InvalidCastException>(() => { foreach (ushort t in rst) {; } });
        }

        [Fact]
        public void EmptySource()
        {
            object[] source = [];
            Assert.Empty(source.Cast<int>());
            Assert.Empty(source.Cast<object, int>());
        }

        [Fact]
        public void NullableIntFromAppropriateObjects()
        {
            int? i = 10;
            object[] source = [-4, 1, 2, 3, 9, i];
            int?[] expected = [-4, 1, 2, 3, 9, i];

            Assert.Equal(expected, source.Cast<int?>());
            Assert.Equal(expected, source.Cast<object, int?>());
        }

        [Fact]
        public void LongFromNullableIntInObjectsThrows()
        {
            int? i = 10;
            object[] source = [-4, 1, 2, 3, 9, i];

            IReadOnlyList<long> cast = source.Cast<long>();
            Assert.Throws<InvalidCastException>(() => cast.ToList());

            cast = source.Cast<object, long>();
            Assert.Throws<InvalidCastException>(() => cast.ToList());
        }

        [Fact]
        public void LongFromNullableIntInObjectsIncludingNullThrows()
        {
            int? i = 10;
            object[] source = [-4, 1, 2, 3, 9, null, i];

            IReadOnlyList<long?> cast = source.Cast<long?>();
            Assert.Throws<InvalidCastException>(() => cast.ToList());

            cast = source.Cast<object, long?>();
            Assert.Throws<InvalidCastException>(() => cast.ToList());
        }

        [Fact]
        public void NullableIntFromAppropriateObjectsIncludingNull()
        {
            int? i = 10;
            object[] source = [-4, 1, 2, 3, 9, null, i];
            int?[] expected = [-4, 1, 2, 3, 9, null, i];

            Assert.Equal(expected, source.Cast<int?>());
            Assert.Equal(expected, source.Cast<object, int?>());
        }

        [Fact]
        public void ThrowOnUncastableItem()
        {
            object[] source = [-4, 1, 2, 3, 9, "45"];
            int[] expectedBeginning = [-4, 1, 2, 3, 9];

            IReadOnlyList<int> cast = source.Cast<int>();
            Assert.Throws<InvalidCastException>(() => cast.ToList());
            Assert.Equal(expectedBeginning, cast.Take(5));
            Assert.Throws<InvalidCastException>(() => cast[5]);

            cast = source.Cast<object, int>();
            Assert.Throws<InvalidCastException>(() => cast.ToList());
            Assert.Equal(expectedBeginning, cast.Take(5));
            Assert.Throws<InvalidCastException>(() => cast[5]);
        }

        [Fact]
        public void ThrowCastingIntToDouble()
        {
            int[] source = [-4, 1, 2, 9];

            IReadOnlyList<double> cast = source.Cast<double>();
            Assert.Throws<InvalidCastException>(() => cast.ToList());

            cast = source.Cast<int, double>();
            Assert.Throws<InvalidCastException>(() => cast.ToList());
        }

        private static void TestCastThrow<T>(object o)
        {
            byte? i = 10;
            object[] source = [-1, 0, o, i];

            IReadOnlyList<T> cast = source.Cast<T>();
            Assert.Throws<InvalidCastException>(() => cast.ToList());

            cast = source.Cast<object, T>();
            Assert.Throws<InvalidCastException>(() => cast.ToList());
        }

        [Fact]
        public void ThrowOnHeterogenousSource()
        {
            TestCastThrow<long?>(null);
            TestCastThrow<long>(9L);
        }

        [Fact]
        public void CastToString()
        {
            object[] source = ["Test1", "4.5", null, "Test2"];
            string[] expected = ["Test1", "4.5", null, "Test2"];

            Assert.Equal(expected, source.Cast<string>());
            Assert.Equal(expected, source.Cast<object, string>());
        }

        [Fact]
        public void ArrayConversionThrows()
        {
            Assert.Throws<InvalidCastException>(() => new[] { -4 }.Cast<long>().ToList());
            Assert.Throws<InvalidCastException>(() => new[] { -4 }.Cast<int, long>().ToList());
        }

        [Fact]
        public void FirstElementInvalidForCast()
        {
            object[] source = ["Test", 3, 5, 10];

            IReadOnlyList<int> cast = source.Cast<int>();
            Assert.Throws<InvalidCastException>(() => cast.ToList());

            cast = source.Cast<object, int>();
            Assert.Throws<InvalidCastException>(() => cast.ToList());
        }

        [Fact]
        public void LastElementInvalidForCast()
        {
            object[] source = [-5, 9, 0, 5, 9, "Test"];

            IReadOnlyList<int> cast = source.Cast<int>();
            Assert.Throws<InvalidCastException>(() => cast.ToList());

            cast = source.Cast<object, int>();
            Assert.Throws<InvalidCastException>(() => cast.ToList());
        }

        [Fact]
        public void NullableIntFromNullsAndInts()
        {
            object[] source = [3, null, 5, -4, 0, null, 9];
            int?[] expected = [3, null, 5, -4, 0, null, 9];

            Assert.Equal(expected, source.Cast<int?>());
            Assert.Equal(expected, source.Cast<object, int?>());
        }

        [Fact]
        public void ThrowCastingIntToLong()
        {
            int[] source = [-4, 1, 2, 3, 9];

            IReadOnlyList<long> cast = source.Cast<long>();
            Assert.Throws<InvalidCastException>(() => cast.ToList());

            cast = source.Cast<int, long>();
            Assert.Throws<InvalidCastException>(() => cast.ToList());
        }

        [Fact]
        public void ThrowCastingIntToNullableLong()
        {
            int[] source = [-4, 1, 2, 3, 9];

            IReadOnlyList<long?> cast = source.Cast<long?>();
            Assert.Throws<InvalidCastException>(() => cast.ToList());

            cast = source.Cast<int, long?>();
            Assert.Throws<InvalidCastException>(() => cast.ToList());
        }

        [Fact]
        public void ThrowCastingNullableIntToLong()
        {
            int?[] source = [-4, 1, 2, 3, 9];

            IReadOnlyList<long> cast = source.Cast<long>();
            Assert.Throws<InvalidCastException>(() => cast.ToList());

            cast = source.Cast<int?, long>();
            Assert.Throws<InvalidCastException>(() => cast.ToList());
        }

        [Fact]
        public void ThrowCastingNullableIntToNullableLong()
        {
            int?[] source = [-4, 1, 2, 3, 9, null];

            IReadOnlyList<long?> cast = source.Cast<long?>();
            Assert.Throws<InvalidCastException>(() => cast.ToList());

            cast = source.Cast<int?, long?>();
            Assert.Throws<InvalidCastException>(() => cast.ToList());
        }

        [Fact]
        public void CastingNullToNonnullableIsNullReferenceException()
        {
            int?[] source = [-4, 1, null, 3];

            IReadOnlyList<int> cast = source.Cast<int>();
            Assert.Throws<NullReferenceException>(() => cast.ToList());

            cast = source.Cast<int?, int>();
            Assert.Throws<NullReferenceException>(() => cast.ToList());
        }

        [Fact]
        public void NullSource()
        {
            AssertExtensions.Throws<ArgumentNullException>("source", () => ((IList)null).Cast<string>());
            AssertExtensions.Throws<ArgumentNullException>("source", () => ((IReadOnlyList<object>)null).Cast<object, string>());
        }

        [Fact]
        public void TargetTypeIsSourceType_Nop()
        {
            object[] values = (string[])["hello", "world"];
            Assert.Same(values, values.Cast<string>());
            Assert.Same(values, values.Cast<object, string>());
        }

        [Fact]
        public void CastOnMultidimensionalArraySucceeds()
        {
            Array array = Array.CreateInstance(typeof(int), 2, 3);
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    array.SetValue((i * 3) + j, i, j);
                }
            }

            int[] result = [.. array.Cast<int>()];
            for (int i = 0; i < 6; i++)
            {
                Assert.Equal(i, result[i]);
            }
        }

        [Fact]
        public void CastCountReturnsExpectedLength()
        {
            object[] objects = ["hello", "world"];
            Assert.Equal(2, objects.Cast<string>().Count);
            Assert.Equal(2, objects.Cast<object, string>().Count);
        }

        [Fact]
        public void CastFirstReturnsFirstElement()
        {
            object[] objects = ["hello", "world"];
            Assert.Equal("hello", objects.Cast<string>()[0]);
            Assert.Equal("hello", objects.Cast<object, string>()[0]);
        }

        [Fact]
        public void CastFirstOnEmptySequenceThrows()
        {
            object[] objects = [];
            Assert.Throws<IndexOutOfRangeException>(() => objects.Cast<string>()[0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => objects.Cast<object, string>()[0]);
        }

        [Fact]
        public void CastLastReturnsLastElement()
        {
            object[] objects = ["hello", "world"];
            Assert.Equal("world", objects.Cast<string>()[^1]);
            Assert.Equal("world", objects.Cast<object, string>()[^1]);
        }

        [Fact]
        public void CastElementAtReturnsExpectedElement()
        {
            object[] objects = ["hello", "world"];
            Assert.Equal("world", objects.Cast<string>()[1]);
            Assert.Equal("world", objects.Cast<object, string>()[1]);
        }

        [Fact]
        public void CastElementAtOutOfRangeThrows()
        {
            object[] objects = ["hello", "world"];
            Assert.Throws<IndexOutOfRangeException>(() => objects.Cast<string>()[2]);
            Assert.Throws<ArgumentOutOfRangeException>(() => objects.Cast<object, string>()[2]);
        }

        [Fact]
        public void CastLastOnEmptySequenceThrows()
        {
            object[] objects = [];
            Assert.Throws<IndexOutOfRangeException>(() => objects.Cast<string>()[^1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => objects.Cast<object, string>()[^1]);
        }

        [Fact]
        public void CastSelectProcessesEachElement()
        {
            object[] objects = ["hello", "world!"];
            Assert.Equal([5, 6], objects.Cast<string>().Select(s => s.Length));
            Assert.Equal([5, 6], objects.Cast<object, string>().Select(s => s.Length));
        }

        [Fact]
        public void CastSkipSkipsElements()
        {
            object[] objects = ["hello", "there", "world"];
            Assert.Equal(["world"], objects.Cast<string>().Skip(2));
            Assert.Equal(["world"], objects.Cast<object, string>().Skip(2));
        }

        [Fact]
        public void CastTakeTakesElements()
        {
            object[] objects = ["hello", "there", "world"];
            Assert.Equal(["hello", "there"], objects.Cast<string>().Take(2));
            Assert.Equal(["hello", "there"], objects.Cast<object, string>().Take(2));
        }
    }
}
