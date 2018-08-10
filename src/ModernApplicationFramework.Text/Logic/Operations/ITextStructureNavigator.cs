using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Text.Logic.Operations
{
    public interface ITextStructureNavigator
    {
        TextExtent GetExtentOfWord(SnapshotPoint currentPosition);

        SnapshotSpan GetSpanOfEnclosing(SnapshotSpan activeSpan);

        SnapshotSpan GetSpanOfFirstChild(SnapshotSpan activeSpan);

        SnapshotSpan GetSpanOfNextSibling(SnapshotSpan activeSpan);

        SnapshotSpan GetSpanOfPreviousSibling(SnapshotSpan activeSpan);

        IContentType ContentType { get; }
    }
}