using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Indexer.Linq.Tests;

public abstract class IndexerableTests
{
    protected class TestReadOnlyList<T>(T[] items) : IReadOnlyList<T>
    {
        public T[] Items = items;
        public int CountTouched = 0;

        public T this[int index] => Items[index];
        public int Count { get { CountTouched++; return Items.Length; } }
        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)Items).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();
    }

    protected static IEnumerable<IReadOnlyList<T>> CreateSources<T>(IReadOnlyList<T> source)
    {
        foreach (Func<IReadOnlyList<T>, IReadOnlyList<T>> t in IdentityTransforms<T>())
        {
            yield return t(source);
        }
    }

    protected static IEnumerable<Func<IReadOnlyList<T>, IReadOnlyList<T>>> IdentityTransforms<T>()
    {
        // Various collection types all representing the same source.
        List<Func<IReadOnlyList<T>, IReadOnlyList<T>>> sources =
        [
            e => e, // original
            e => (T[])[.. e], // T[]
            e => (List<T>)[.. e], // List<T>
            e => new ReadOnlyCollection<T>([.. e]), // IList<T> that's not List<T>/T[]
        ];
        if (typeof(T) == typeof(char))
        {
            sources.Add(e => (IReadOnlyList<T>)(object)string.Concat((IReadOnlyList<char>)(object)e).ToReadOnlyList()); // string
        }

        // Various transforms that all yield the same elements as the source.
        List<Func<IReadOnlyList<T>, IReadOnlyList<T>>> transforms =
        [
            // Append
            e =>
            {
                T[] values = [.. e];
                return values.Length == 0 ? [] : values[0..^1].Append(values[^1]);
            },

            // Concat
            e => e.Concat([]),
            e => ((T[])[]).Concat(e),

            // Prepend
            e =>
            {
                T[] values = [.. e];
                return values.Length == 0 ? [] : values[1..].Prepend(values[0]);
            },

            // Reverse
            e => e.Reverse().Reverse(),

            // Select
            e => e.Select(i => i),

            // Take
            e => e.Take(int.MaxValue),
            e => e.TakeLast(int.MaxValue)
        ];

        foreach (Func<IReadOnlyList<T>, IReadOnlyList<T>> source in sources)
        {
            // Yield the source itself.
            yield return source;

            foreach (Func<IReadOnlyList<T>, IReadOnlyList<T>> transform in transforms)
            {
                // Yield a single transform on the source
                yield return e => transform(source(e));

                foreach (Func<IReadOnlyList<T>, IReadOnlyList<T>> transform2 in transforms)
                {
                    // Yield a second transform on the first transform on the source.
                    yield return e => transform2(transform(source(e)));
                }
            }
        }
    }
}
