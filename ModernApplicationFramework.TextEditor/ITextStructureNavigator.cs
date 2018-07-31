namespace ModernApplicationFramework.TextEditor
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