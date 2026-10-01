using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Indexer.Linq.Tests
{
    public class SkipTakeData
    {
        public static IEnumerable<TheoryDataRow<IReadOnlyList<int>, int>> IndexerableData()
        {
            IReadOnlyList<int> sourceCounts = [0, 1, 2, 3, 5, 8, 13, 55, 100, 250];

            IReadOnlyList<int> counts = [1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 100, 250, 500, int.MaxValue];
            counts = counts.Concat(counts.Select(c => -c)).Append(0).Append(int.MinValue);

            return from sourceCount in sourceCounts
                   let source = Indexerable.Range(0, sourceCount)
                   from count in counts
                   select new TheoryDataRow<IReadOnlyList<int>, int>(source, count);
        }
    }
}
