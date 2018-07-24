using System;
using ModernApplicationFramework.TextEditor.Text.Differencing;

namespace ModernApplicationFramework.TextEditor.Text
{
    public interface ITextDifferencingService
    {
        IHierarchicalDifferenceCollection DiffStrings(string left, string right, StringDifferenceOptions differenceOptions);

        IHierarchicalDifferenceCollection DiffSnapshotSpans(SnapshotSpan left, SnapshotSpan right, StringDifferenceOptions differenceOptions);

        IHierarchicalDifferenceCollection DiffSnapshotSpans(SnapshotSpan left, SnapshotSpan right, StringDifferenceOptions differenceOptions, Func<ITextSnapshotLine, string> getLineTextCallback);
    }
}