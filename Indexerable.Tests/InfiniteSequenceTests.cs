using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using Xunit;

namespace Indexer.Linq.Tests
{
    public class InfiniteSequenceTests : IndexerableTests
    {
        [Fact]
        public void NullArguments_Throws()
        {
            AssertExtensions.Throws<ArgumentNullException>("start", () => Indexerable.InfiniteSequence<ReferenceAddable>(null!, new()));
            AssertExtensions.Throws<ArgumentNullException>("step", () => Indexerable.InfiniteSequence<ReferenceAddable>(new(), null!));
        }

        [Fact]
        public void MultipleGetEnumeratorCalls_ReturnsUniqueInstances()
        {
            IReadOnlyList<int> sequence = Indexerable.InfiniteSequence(0, 1);

            IEnumerator<int> enumerator1 = sequence.GetEnumerator();
            IEnumerator<int> enumerator2 = sequence.GetEnumerator();
            Assert.NotSame(enumerator1, enumerator2);
            enumerator1.Dispose();
            enumerator2.Dispose();
        }

        [Fact]
        public void InfiniteSequence_AllZeroes_MatchesExpectedOutput()
        {
            Assert.Equal(Indexerable.Repeat(0, 10), Indexerable.InfiniteSequence(0, 0).Take(10));
            Assert.Equal(Indexerable.Repeat(0, 10).Select(i => (char)i), Indexerable.InfiniteSequence((char)0, (char)0).Take(10));
            Assert.Equal(Indexerable.Repeat(0, 10).Select(i => (BigInteger)i), Indexerable.InfiniteSequence(BigInteger.Zero, BigInteger.Zero).Take(10));
            Assert.Equal(Indexerable.Repeat(0, 10).Select(i => (float)i), Indexerable.InfiniteSequence((float)0, 0).Take(10));
        }

        [Fact]
        public void InfiniteSequence_NegativeStepContainsAndFindsValues()
        {
            IList<int> sequence = (IList<int>)Indexerable.InfiniteSequence(10, -1);

            Assert.True(sequence.Contains(9));
            Assert.Equal(1, sequence.IndexOf(9));
            Assert.True(sequence.Contains(6));
            Assert.Equal(4, sequence.IndexOf(6));
            Assert.False(sequence.Contains(11));
            Assert.Equal(-1, sequence.IndexOf(11));
        }

        [Fact]
        public void InfiniteSequence_ZeroStepContainsOnlyRepeatedValue()
        {
            IList<int> sequence = (IList<int>)Indexerable.InfiniteSequence(10, 0);

            Assert.True(sequence.Contains(10));
            Assert.Equal(0, sequence.IndexOf(10));
            Assert.False(sequence.Contains(9));
            Assert.Equal(-1, sequence.IndexOf(9));
        }

        [Fact]
        public void InfiniteSequence_ProducesExpectedSequence()
        {
            Validate<sbyte>(0, 1);
            Validate<sbyte>(sbyte.MaxValue - 3, 2);
            Validate<sbyte>(sbyte.MinValue, sbyte.MaxValue / 2);

            Validate(0, 1);
            Validate(4, -3);
            Validate(int.MaxValue - 3, 2);
            Validate(int.MinValue, int.MaxValue / 2);

            Validate(0L, 1L);
            Validate(-4L, -3L);
            Validate(long.MaxValue - 3L, 2L);
            Validate(long.MinValue, long.MaxValue / 2L);

            Validate(0f, 1f);
            Validate(0f, -1f);
            Validate(float.MaxValue, 1f);
            Validate(float.MinValue, float.MaxValue / 2f);

            Validate(new BigInteger(long.MaxValue) * 3, (BigInteger)12345);
            Validate(new BigInteger(long.MaxValue) * 3, (BigInteger)(-12345));

            static void Validate<T>(T start, T step) where T : INumber<T>
            {
                IReadOnlyList<T> sequence = Indexerable.InfiniteSequence(start, step);

                for (int trial = 0; trial < 2; trial++)
                {
                    T expected = start;
                    for (int i = 0; i < 10; i++)
                    {
                        Assert.Equal(expected, sequence[i]);

                        expected += step;
                    }
                }
            }
        }

#nullable enable
        private sealed class ReferenceAddable(int value = 0) : INumber<ReferenceAddable>
        {
            public static ReferenceAddable One => throw new NotImplementedException();
            public static int Radix => throw new NotImplementedException();
            public static ReferenceAddable Zero => throw new NotImplementedException();
            public static ReferenceAddable AdditiveIdentity => throw new NotImplementedException();
            public static ReferenceAddable MultiplicativeIdentity => throw new NotImplementedException();
            public static ReferenceAddable Abs(ReferenceAddable value) => throw new NotImplementedException();
            public static bool IsCanonical(ReferenceAddable value) => throw new NotImplementedException();
            public static bool IsComplexNumber(ReferenceAddable value) => throw new NotImplementedException();
            public static bool IsEvenInteger(ReferenceAddable value) => throw new NotImplementedException();
            public static bool IsFinite(ReferenceAddable value) => throw new NotImplementedException();
            public static bool IsImaginaryNumber(ReferenceAddable value) => throw new NotImplementedException();
            public static bool IsInfinity(ReferenceAddable value) => throw new NotImplementedException();
            public static bool IsInteger(ReferenceAddable value) => throw new NotImplementedException();
            public static bool IsNaN(ReferenceAddable value) => false;
            public static bool IsNegative(ReferenceAddable value) => false;
            public static bool IsNegativeInfinity(ReferenceAddable value) => throw new NotImplementedException();
            public static bool IsNormal(ReferenceAddable value) => throw new NotImplementedException();
            public static bool IsOddInteger(ReferenceAddable value) => throw new NotImplementedException();
            public static bool IsPositive(ReferenceAddable value) => false;
            public static bool IsPositiveInfinity(ReferenceAddable value) => throw new NotImplementedException();
            public static bool IsRealNumber(ReferenceAddable value) => throw new NotImplementedException();
            public static bool IsSubnormal(ReferenceAddable value) => throw new NotImplementedException();
            public static bool IsZero(ReferenceAddable value) => false;
            public static ReferenceAddable MaxMagnitude(ReferenceAddable x, ReferenceAddable y) => throw new NotImplementedException();
            public static ReferenceAddable MaxMagnitudeNumber(ReferenceAddable x, ReferenceAddable y) => throw new NotImplementedException();
            public static ReferenceAddable MinMagnitude(ReferenceAddable x, ReferenceAddable y) => throw new NotImplementedException();
            public static ReferenceAddable MinMagnitudeNumber(ReferenceAddable x, ReferenceAddable y) => throw new NotImplementedException();
            public static ReferenceAddable Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) => throw new NotImplementedException();
            public static ReferenceAddable Parse(string s, NumberStyles style, IFormatProvider? provider) => throw new NotImplementedException();
            public static ReferenceAddable Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => throw new NotImplementedException();
            public static ReferenceAddable Parse(string s, IFormatProvider? provider) => throw new NotImplementedException();
            public static bool TryConvertFromChecked<TOther>(TOther value, [MaybeNullWhen(false)] out ReferenceAddable result) where TOther : INumberBase<TOther> => throw new NotImplementedException();
            public static bool TryConvertFromSaturating<TOther>(TOther value, [MaybeNullWhen(false)] out ReferenceAddable result) where TOther : INumberBase<TOther> => throw new NotImplementedException();
            public static bool TryConvertFromTruncating<TOther>(TOther value, [MaybeNullWhen(false)] out ReferenceAddable result) where TOther : INumberBase<TOther> => throw new NotImplementedException();
            public static bool TryConvertToChecked<TOther>(ReferenceAddable value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther> => throw new NotImplementedException();
            public static bool TryConvertToSaturating<TOther>(ReferenceAddable value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther> => throw new NotImplementedException();
            public static bool TryConvertToTruncating<TOther>(ReferenceAddable value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther> => throw new NotImplementedException();
            public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out ReferenceAddable result) => throw new NotImplementedException();
            public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out ReferenceAddable result) => throw new NotImplementedException();
            public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out ReferenceAddable result) => throw new NotImplementedException();
            public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out ReferenceAddable result) => throw new NotImplementedException();
            public int CompareTo(object? obj) => throw new NotImplementedException();
            public int CompareTo(ReferenceAddable? other) => throw new NotImplementedException();
            public bool Equals(ReferenceAddable? other) => throw new NotImplementedException();
            public string ToString(string? format, IFormatProvider? formatProvider) => value.ToString(format, formatProvider);
            public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => throw new NotImplementedException();
            public static ReferenceAddable operator +(ReferenceAddable value) => throw new NotImplementedException();
            public static ReferenceAddable operator +(ReferenceAddable left, ReferenceAddable right) => throw new NotImplementedException();
            public static ReferenceAddable operator -(ReferenceAddable value) => throw new NotImplementedException();
            public static ReferenceAddable operator -(ReferenceAddable left, ReferenceAddable right) => throw new NotImplementedException();
            public static ReferenceAddable operator ++(ReferenceAddable value) => throw new NotImplementedException();
            public static ReferenceAddable operator --(ReferenceAddable value) => throw new NotImplementedException();
            public static ReferenceAddable operator *(ReferenceAddable left, ReferenceAddable right) => throw new NotImplementedException();
            public static ReferenceAddable operator /(ReferenceAddable left, ReferenceAddable right) => throw new NotImplementedException();
            public static ReferenceAddable operator %(ReferenceAddable left, ReferenceAddable right) => throw new NotImplementedException();
            public static bool operator ==(ReferenceAddable? left, ReferenceAddable? right) => throw new NotImplementedException();
            public static bool operator !=(ReferenceAddable? left, ReferenceAddable? right) => throw new NotImplementedException();
            public static bool operator <(ReferenceAddable left, ReferenceAddable right) => throw new NotImplementedException();
            public static bool operator >(ReferenceAddable left, ReferenceAddable right) => throw new NotImplementedException();
            public static bool operator <=(ReferenceAddable left, ReferenceAddable right) => throw new NotImplementedException();
            public static bool operator >=(ReferenceAddable left, ReferenceAddable right) => throw new NotImplementedException();
            public override bool Equals(object? obj) => throw new NotImplementedException();
            public override int GetHashCode() => throw new NotImplementedException();
        }
    }
}
