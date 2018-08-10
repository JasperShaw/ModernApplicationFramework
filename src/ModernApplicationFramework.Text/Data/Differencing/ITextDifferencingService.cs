using System;

namespace ModernApplicationFramework.Text.Data.Differencing
{
    public interface ITextDifferencingService
    {
        IHierarchicalDifferenceCollection DiffSnapshotSpans(SnapshotSpan left, SnapshotSpan right,
            StringDifferenceOptions differenceOptions);

        IHierarchicalDifferenceCollection DiffSnapshotSpans(SnapshotSpan left, SnapshotSpan right,
            StringDifferenceOptions differenceOptions, Func<ITextSnapshotLine, string> getLineTextCallback);

        IHierarchicalDifferenceCollection DiffStrings(string left, string right,
            StringDifferenceOptions differenceOptions);
    }
}