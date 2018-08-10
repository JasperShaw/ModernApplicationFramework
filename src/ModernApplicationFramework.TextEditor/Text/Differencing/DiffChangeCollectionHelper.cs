using System.Collections.Generic;
using ModernApplicationFramework.Text.Data.Differencing;

namespace ModernApplicationFramework.TextEditor.Text.Differencing
{
    internal static class DiffChangeCollectionHelper<T>
    {
        public static DifferenceCollection<T> Create(IDiffChange[] changes, IList<T> originalLeft, IList<T> originalRight)
        {
            return new DifferenceCollection<T>(CreateDiffs(changes, originalLeft, originalRight), originalLeft, originalRight);
        }

        private static IList<Difference> CreateDiffs(IDiffChange[] changes, IList<T> originalLeft, IList<T> originalRight)
        {
            IList<Difference> diffs = new List<Difference>(changes.Length);
            if (changes.Length != 0)
            {
                var diffChange = changes[0];
                var initialMatch = DifferenceCollection<T>.CreateInitialMatch(diffChange.OriginalStart);
                for (var index = 1; index < changes.Length; ++index)
                {
                    var change = changes[index];
                    DifferenceCollection<T>.AddDifference(diffChange.OriginalStart, diffChange.OriginalEnd, change.OriginalStart, diffChange.ModifiedStart, diffChange.ModifiedEnd, change.ModifiedStart, diffs, ref initialMatch);
                    diffChange = change;
                }
                DifferenceCollection<T>.AddDifference(diffChange.OriginalStart, diffChange.OriginalEnd, originalLeft.Count, diffChange.ModifiedStart, diffChange.ModifiedEnd, originalRight.Count, diffs, ref initialMatch);
            }
            return diffs;
        }
    }
}